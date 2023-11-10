using Newtonsoft.Json;
using System;
using System.Xml.Linq;
using System.Xml.XPath;
using TorunLive.Domain.Entities;
using TorunLive.Domain.EntitiesV2;
using TorunLive.Persistance;

namespace TorunLive.SIPTimetableScanner
{
    public class TimetableScannerService
    {
        private readonly TorunLiveContext _dbContext;
        private readonly HttpClient _httpClient;
        private static readonly string[] _lines = new[]
        {
            "1", "2", "3", "4", "5", "6", "7",
            "10", "11", "12", "13", "14", "15", "16", "17",
            "18", "19", "20", "24", "25", "26", "27", "28",
            "29", "30", "31", "32", "33", "34", "38", "39",
            "40", "41", "42", "44", "111", "112", "113", "115",
            "121", "122", "131", "N90", "N91", "N93", "N94", "N95"
        };

        public TimetableScannerService(TorunLiveContext dbContext)
        {
            _dbContext = dbContext;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://rozklad.mzk-torun.pl/")
            };
        }

        public async Task ScanTimetablesAndLines()
        {
            var random = new Random();
            var allLinesAndDirections = new List<LineEntry>();
            try
            {
                foreach (var line in _lines)
                {
                    Console.WriteLine($"Scanning line {line}");
                    var directions = await GetLineEntries(line);
                    var lineUrls = directions.Select(d => d.TimetableUrl).ToList();

                    foreach (var direction in directions)
                    {
                        await ScanLineDirectionStopAndStopTimes(direction, lineUrls);
                    }

                    var delay = (int)(250 * random.NextDouble());
                    await Task.Delay(delay);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public async Task ScanLineDirectionStopAndStopTimes(LineEntry direction, List<string> lineUrls)
        {
            Console.WriteLine($"Found '{direction.DirectionName}', scanning stops...");
            var secondLineUrl = lineUrls.Except(new[] { direction.TimetableUrl }).First();
            var timetableStopUrlsAndNames = await GetTimetableStopUrls(direction.TimetableUrl);
            int index = 0;

            // add direction to db

            var urlData = direction.TimetableUrl.Split('/');
            var lineId = urlData[0];
            var directionId = int.Parse(urlData[1]);

            foreach (var (stopUrl, stopName) in timetableStopUrlsAndNames)
            {
                var stopId = stopUrl.Replace(".html", "");
                if (!_dbContext.Stops.Any(s => s.Id == stopId))
                {
                    var fixedName = stopName.Replace("nż.", "");
                    _dbContext.Stops.Add(new Domain.EntitiesV2.Stop { Id = stopId, Name = fixedName });
                }

                var lineStop = _dbContext.LineStops.Add(new LineStop
                {
                    LineId = lineId,
                    DirectionId = directionId,
                    StopId = stopId,
                    StopOrder = index++,
                    IsOnDemand = stopName.Contains("nż.")
                    //TimeToNextStop = 1
                }).Entity;
                _dbContext.SaveChanges();

                var timetableUrl = $"{lineId}/{directionId}/{stopId}.html";
                var response = await _httpClient.GetAsync(timetableUrl);
                response.EnsureSuccessStatusCode();
                var rawData = await response.Content.ReadAsStringAsync();

                var stopTimesDelay = (int)(200 * new Random().NextDouble());
                await Task.Delay(stopTimesDelay);

                ReadAndInsertArrivalTimes(lineStop.Id, rawData);
            }

            var innerDelay = (int)(200 * new Random().NextDouble());
            await Task.Delay(innerDelay);
        }

        public async Task<List<(string, string)>> GetTimetableStopUrls(string targetLineUrl)
        {
            var response = await _httpClient.GetAsync(targetLineUrl);
            response.EnsureSuccessStatusCode();

            var rawData = await response.Content.ReadAsStringAsync();
            var parsed = ParseTimetablesUrls(rawData).ToList();
            var fixedFirstItem = (targetLineUrl.Split('/')[2], parsed[0].Item2);
            parsed[0] = fixedFirstItem;
            return parsed;
        }

        private static string GetTextBetweenAndClean(string text, string start, string end, Dictionary<string, string> elementsToReplace)
        {
            var startIndex = text.IndexOf(start);
            var endIndex = text.IndexOf(end);
            var substring = text.Substring(startIndex, endIndex - startIndex);
            foreach(var replacement in elementsToReplace)
            {
                substring = substring.Replace(replacement.Key, replacement.Value);
            }

            return substring;
        }

        public static IEnumerable<(string, string)> ParseTimetablesUrls(string lineData)
        {
            var start = "<div class=\"timetable-stops\">";
            var endMarker = "<div class=\"timetable\">";
            var replacements = new Dictionary<string, string>
            {
                { "<br>", "<br></br>" },
                { "&", "&amp;" }
            };
            var substring = GetTextBetweenAndClean(lineData, start, endMarker, replacements);

            var document = XDocument.Parse(substring);
            var urls = document.XPathSelectElements("div/div/table/tr/td/a").ToList();
            var times = document.XPathSelectElements("div/div/table/tr/td")
                .Where(t => t.Attributes().Any(a => a.Value == "czas")).ToList();

            foreach (var (url, time) in urls.Zip(times))
            {
                var lineStopUrl = url.Attributes().FirstOrDefault(a => a.Name == "href")?.Value ?? string.Empty;
                yield return (lineStopUrl, url.Value);
            }
        }

        public void ReadAndInsertArrivalTimes(int lineStopId, string lineData)
        {
            var start = "<table cellspacing=\"0\" cellpadding=\"0\" id=\"tab_roz_godz\" style=\"\">";
            var endMarker = "</div></div><div class=\"timetable-footer\">";
            var replacements = new Dictionary<string, string>
            {
                { "<br>", "<br></br>" },
                { "&", "&amp;" }
            };

            var substring = GetTextBetweenAndClean(lineData, start, endMarker, replacements);
            var document = XDocument.Parse(substring);
            var tableRows = document.XPathSelectElements("table/tbody/tr").ToList();

            // skip header row
            var lineStopTimesToAdd = new List<LineStopTime>();
            foreach (var row in tableRows.Skip(1))
            {
                var hoursAndMinutes = row.Descendants().ToList();
                // there's always 6 entries
                // first HH, first mm - weekdays
                // second HH, second mm - winter holidays
                // third HH, third mm - saturday, sunday, holidays

                // sometimes there can be four columns - so 8 entries
                // first HH, first mm - weekdays
                // second HH, second mm - winter holidays
                // third HH, third mm - saturday, sunday
                // fourth HH, fourth mm - holidays
                lineStopTimesToAdd.AddRange(GetArrivalsDayMinutes(hoursAndMinutes[0], hoursAndMinutes[1])
                    .Select(a => new LineStopTime
                    {
                        LineStopId = lineStopId,
                        DayMinute = a,
                        IsWeekday = true
                    }));
                lineStopTimesToAdd.AddRange(GetArrivalsDayMinutes(hoursAndMinutes[2], hoursAndMinutes[3])
                    .Select(a => new LineStopTime
                    {
                        LineStopId = lineStopId,
                        DayMinute = a,
                        IsWinterHoliday = true
                    }));
                lineStopTimesToAdd.AddRange(GetArrivalsDayMinutes(hoursAndMinutes[4], hoursAndMinutes[5])
                    .Select(a => new LineStopTime
                    {
                        LineStopId = lineStopId,
                        DayMinute = a,
                        IsSaturdaySundayHoliday = true
                    }));
            }

            _dbContext.LineStopTimes.AddRange(lineStopTimesToAdd);
            _dbContext.SaveChanges();
        }

        private List<int> GetArrivalsDayMinutes(XElement hourCell, XElement minuteCell)
        {
            var dayminutes = new List<int>();

            var hour = hourCell.Value;
            if (string.IsNullOrEmpty(hour))
                return dayminutes;

            var parsedHour = int.Parse(hour);
            var minutes = minuteCell.Value.Split('.');
            foreach (var minute in minutes)
            {
                var parsedMinute = int.Parse(minute.Replace("^", "").Replace("a", "").Replace("d", ""));
                var dayMinuteArrival = parsedHour * 60 + parsedMinute;
                dayminutes.Add(dayMinuteArrival);
            }

            return dayminutes;
        }

        public async Task<IEnumerable<LineEntry>> GetLineEntries(string lineName)
        {
            var response = await _httpClient.GetAsync($"Linia{lineName}.html");
            response.EnsureSuccessStatusCode();

            var rawData = await response.Content.ReadAsStringAsync();
            var parsed = GetLinesDirections(lineName, rawData);
            return parsed;
        }

        public static IEnumerable<LineEntry> GetLinesDirections(string name, string lineData)
        {
            var start = "<center><p>";
            var endMarker = "</a></center>";
            var replacements = new Dictionary<string, string>
            {
                { "<br>", "<br></br>" },
                { "&", "&amp;" }
            };

            var substring = GetTextBetweenAndClean(lineData, start, endMarker, replacements);
            var ignoreUrls = new[]
            {
                "panel.html", "https://mzk-torun.pl"
            };

            var document = XDocument.Parse(substring);
            var urls = document.XPathSelectElements("center/a").ToList();
            var entries = new List<LineEntry>();
            foreach (var url in urls)
            {
                var attributes = url.Attributes();
                var hrefAttribute = attributes.FirstOrDefault(a => a.Name == "href")?.Value ?? null;
                if (ignoreUrls.Contains(hrefAttribute))
                    continue;

                var aValue = url.Value.Replace("<h3>", "").Replace("</h3>", "");
                yield return new LineEntry
                {
                    Name = name,
                    DirectionName = aValue,
                    TimetableUrl = hrefAttribute
                };
            }
        }
    }
}
