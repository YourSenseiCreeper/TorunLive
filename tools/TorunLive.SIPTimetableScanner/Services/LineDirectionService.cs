using System.Xml.Linq;
using System.Xml.XPath;
using TorunLive.Domain.EntitiesV2;
using TorunLive.Persistance;

namespace TorunLive.SIPTimetableScanner.Services
{
    public class LineDirectionService
    {
        private readonly TorunLiveContext _dbContext;
        private readonly HttpClient _httpClient;

        public LineDirectionService(HttpClient httpClient, TorunLiveContext dbContext)
        {
            _httpClient = httpClient;
            _dbContext = dbContext;
        }

        public async Task ScanLineDirectionStopAndStopTimes(Entities.LineDirection direction, List<string> lineUrls)
        {
            Console.WriteLine($"Found '{direction.DirectionName}', scanning stops...");
            var secondLineUrl = lineUrls.Except(new[] { direction.Url }).First();
            var timetableStopUrlsAndNames = await GetTimetableStopUrls(direction.Url);
            int index = 0;

            // add direction to db

            var urlData = direction.Url.Split('/');
            var lineId = urlData[0];
            var directionId = int.Parse(urlData[1]);

            foreach (var (stopUrl, stopName) in timetableStopUrlsAndNames)
            {
                var stopId = stopUrl.Replace(".html", "");
                if (!_dbContext.Stops.Any(s => s.Id == stopId))
                {
                    var fixedName = stopName.Replace("nż.", "");
                    _dbContext.Stops.Add(new Stop { Id = stopId, Name = fixedName });
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

        public static IEnumerable<(string, string)> ParseTimetablesUrls(string lineData)
        {
            var substring = Common.GetTextBetweenAndClean(lineData, "<div class=\"timetable-stops\">", "<div class=\"timetable\">", Common.XmlEscapeReplacements);

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

        public void ReadAndInsertArrivalTimes(int lineStopId, string lineData)
        {
            var substring = Common.GetTextBetweenAndClean(
                lineData,
                "<table cellspacing=\"0\" cellpadding=\"0\" id=\"tab_roz_godz\" style=\"\">",
                "</div></div><div class=\"timetable-footer\">"
                , Common.XmlEscapeReplacements
            );
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

        private static IEnumerable<int> GetArrivalsDayMinutes(XElement hourCell, XElement minuteCell)
        {
            var hour = hourCell.Value;
            if (string.IsNullOrEmpty(hour))
                yield break;

            var parsedHour = int.Parse(hour);
            var minutes = minuteCell.Value.Split('.');
            foreach (var minute in minutes)
            {
                var parsedMinute = int.Parse(minute.Replace("^", "").Replace("a", "").Replace("d", ""));
                yield return parsedHour * 60 + parsedMinute;
            }
        }
    }
}
