using Newtonsoft.Json;
using System.Linq;
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
            var lines = new[]
            {
                "1", "2", "3", "4", "5", "6", "7",
                "10", "11", "12", "13", "14", "15", "16", "17",
                "18", "19", "20", "24", "25", "26", "27", "28",
                "29", "30", "31", "32", "33", "34", "38", "39",
                "40", "41", "42", "44", "111", "112", "113", "115",
                "121", "122", "131", "N90", "N91", "N93", "N94", "N95"
            };

            string fileName = "timetables.json";
            var random = new Random();
            var allLinesAndDirections = new List<LineEntry>();
            try
            {
                foreach (var line in lines)
                {
                    Console.WriteLine($"Scanning line {line}");
                    var directions = await GetLineEntries(line);
                    var lineUrls = directions.Select(d => d.TimetableUrl).ToList();

                    foreach (var direction in directions)
                    {
                        Console.WriteLine($"Found '{direction.DirectionName}', scanning stops...");
                        var timetableStopUrls = await GetTimetableStopUrls(direction.TimetableUrl);
                        var secondLineUrl = lineUrls.Except(new[] { direction.TimetableUrl }).First();
                        int index = 0;

                        var urlData = direction.TimetableUrl.Split('/');
                        var lineId = urlData[0];
                        var directionId = int.Parse(urlData[1]);

                        foreach (var stopUrl in timetableStopUrls)
                        {
                            var stopId = stopUrl.Replace(".html", "");
                            if (!_dbContext.Stops.Any(s => s.Id == stopId))
                            {
                                // Name cannot be null - need to return Dictionary<Id, Name>
                                _dbContext.Stops.Add(new Domain.EntitiesV2.Stop { Id = stopId });
                            }

                            var lineStop = _dbContext.LineStops.Add(new LineStop
                            {
                                LineId = lineId,
                                DirectionId = directionId,
                                StopId = stopId,
                                StopOrder = index++,
                                //IsOnDemand = false
                                //TimeToNextStop = 1
                            }).Entity;
                            _dbContext.SaveChanges();

                            var timetableUrl = $"{lineId}/{directionId}/{stopId}.html";
                            var response = await _httpClient.GetAsync(timetableUrl);
                            response.EnsureSuccessStatusCode();
                            var rawData = await response.Content.ReadAsStringAsync();
                            ReadAndInsertArrivalTimes(lineStop.Id, secondLineUrl, rawData);
                        }

                        //direction.Timetable = timetableStopUrls;
                        allLinesAndDirections.Add(direction);
                        var innerDelay = (int)(100 * random.NextDouble());
                        await Task.Delay(innerDelay);
                    }

                    SaveToFile(fileName, allLinesAndDirections);
                    var delay = (int)(250 * random.NextDouble());
                    await Task.Delay(delay);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                SaveToFile(fileName, allLinesAndDirections);
            }
        }

        public async Task<List<string>> GetTimetableStopUrls(string targetLineUrl)
        {
            var response = await _httpClient.GetAsync(targetLineUrl);
            response.EnsureSuccessStatusCode();

            var rawData = await response.Content.ReadAsStringAsync();
            var parsed = ParseTimetablesUrls(rawData);
            parsed[0] = targetLineUrl.Split('/')[2];
            return parsed;
        }

        public static List<string> ParseTimetablesUrls(string lineData)
        {
            var start = "<div class=\"timetable-stops\">";
            var startIndex = lineData.IndexOf(start);
            var endMarker = "<div class=\"timetable\">";
            var endIndex = lineData.IndexOf(endMarker);
            var substring = lineData.Substring(startIndex, endIndex - startIndex);
            substring = substring.Replace("<br>", "<br></br>").Replace("&", "&amp;");

            var document = XDocument.Parse(substring);
            var urls = document.XPathSelectElements("div/div/table/tr/td/a").ToList();
            var times = document.XPathSelectElements("div/div/table/tr/td")
                .Where(t => t.Attributes().Any(a => a.Value == "czas")).ToList();

            var entries = new List<string>();
            foreach (var (url, time) in urls.Zip(times))
            {
                var lineStopUrl = url.Attributes().FirstOrDefault(a => a.Name == "href")?.Value ?? string.Empty;
                //lineStopUrl = lineStopUrl.Replace(".html", "");
                entries.Add(lineStopUrl);
            }

            return entries;
        }

        public static List<TimetableEntry> ParseTimetableData(string lineData)
        {
            var start = "<div class=\"timetable-stops\">";
            var startIndex = lineData.IndexOf(start);
            var endMarker = "<div class=\"timetable\">";
            var endIndex = lineData.IndexOf(endMarker);
            var substring = lineData.Substring(startIndex, endIndex - startIndex);
            substring = substring.Replace("<br>", "<br></br>").Replace("&", "&amp;");

            var document = XDocument.Parse(substring);
            var urls = document.XPathSelectElements("div/div/table/tr/td/a").ToList();
            var times = document.XPathSelectElements("div/div/table/tr/td")
                .Where(t => t.Attributes().Any(a => a.Value == "czas")).ToList();

            var entries = new List<TimetableEntry>();
            foreach (var (url, time) in urls.Zip(times))
            {
                var lineStopUrl = url.Attributes().FirstOrDefault(a => a.Name == "href")?.Value ?? string.Empty;
                lineStopUrl = lineStopUrl.Replace(".html", "");
                int stopId = string.IsNullOrEmpty(lineStopUrl) ? 0 : int.Parse(lineStopUrl);
                if (!int.TryParse(time.Value, out int timeElapsedFromFirstStop))
                {
                    Console.WriteLine($"Cannot parse timeElapsed value: '{time.Value}'");
                    timeElapsedFromFirstStop = 0;
                }
                entries.Add(new TimetableEntry
                {
                    Name = url.Value.Replace("&amp;", "&"),
                    StopId = stopId,
                    TimeElapsedFromFirstStop = timeElapsedFromFirstStop
                });
            }

            // ostatnią trzeba dodać manualnie, bo nie ma linku
            var cells = document.XPathSelectElements("div/div/table/tr/td").ToList();
            var lastCell = cells.LastOrDefault();
            if (lastCell != null)
            {
                entries.Add(new TimetableEntry
                {
                    Name = lastCell.Value.Replace("&amp;", "&"),
                });
            }
            return entries;
        }

        public void ReadAndInsertArrivalTimes(int lineStopId, string secondLineUrl, string lineData)
        {
            var entries = new List<TimetableEntry>();
            var start = "<table cellspacing=\"0\" cellpadding=\"0\" id=\"tab_roz_godz\" style=\"\">";
            var startIndex = lineData.IndexOf(start);
            var endMarker = "</div></div><div class=\"timetable-footer\">";
            var endIndex = lineData.IndexOf(endMarker);
            var substring = lineData.Substring(startIndex, endIndex - startIndex);
            substring = substring.Replace("&", "&amp;"); // uważać na ^ ???

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

        public async Task<List<LineEntry>> GetLineEntries(string lineName)
        {
            var response = await _httpClient.GetAsync($"Linia{lineName}.html");
            response.EnsureSuccessStatusCode();

            var rawData = await response.Content.ReadAsStringAsync();
            var parsed = GetLinesDirections(lineName, rawData);
            return parsed;
        }

        public static List<LineEntry> GetLinesDirections(string name, string lineData)
        {
            var entries = new List<LineEntry>();
            var start = "<center><p>";
            var startIndex = lineData.IndexOf(start);
            var endMarker = "</a></center>";
            var endIndex = lineData.IndexOf(endMarker);
            var substring = lineData.Substring(startIndex, endIndex + endMarker.Length - startIndex);
            substring = substring.Replace("<br>", "<br></br>").Replace("&", "&amp;");

            var ignoreUrls = new[]
            {
                "panel.html", "https://mzk-torun.pl"
            };

            var document = XDocument.Parse(substring);
            var urls = document.XPathSelectElements("center/a").ToList();
            foreach(var url in urls)
            {
                var attributes = url.Attributes();
                var hrefAttribute = attributes.FirstOrDefault(a => a.Name == "href")?.Value ?? null;
                if (ignoreUrls.Contains(hrefAttribute))
                    continue;

                var aValue = url.Value.Replace("<h3>", "").Replace("</h3>", "");
                entries.Add(new LineEntry
                {
                    Name = name,
                    DirectionName = aValue,
                    TimetableUrl = hrefAttribute
                });
            }
            return entries;
        }

        public static void SaveToFile(string filename, List<LineEntry> lineEntries)
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var path = Path.Combine(currentDirectory, filename);
            var serialized = JsonConvert.SerializeObject(lineEntries);
            File.WriteAllText(path, serialized);
        }
    }
}
