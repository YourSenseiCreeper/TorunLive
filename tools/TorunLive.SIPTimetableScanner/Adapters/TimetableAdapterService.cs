using System.Xml.Linq;
using System.Xml.XPath;
using TorunLive.Domain.EntitiesV2;
using TorunLive.SIPTimetableScanner.Entities;
using TorunLive.SIPTimetableScanner.Interfaces.Adapters;

namespace TorunLive.SIPTimetableScanner.Adapters
{
    public class TimetableAdapterService : ITimetableAdapterService
    {
        public TimetableAdapterService() { }

        public IEnumerable<LineStopTime> ParseArrivals(int lineStopId, string lineData)
        {
            var substring = Common.GetTextBetweenAndClean(
                lineData,
                "<table cellspacing=\"0\" cellpadding=\"0\" id=\"tab_roz_godz\" style=\"\">",
                "</div></div><div class=\"timetable-footer\">"
                , Common.XmlEscapeReplacements
            );
            var document = XDocument.Parse(substring);
            var tableRows = document.XPathSelectElements("table/tbody/tr").ToList();

            var lineStopTimesToAdd = new List<LineStopTime>();
            // skip header row
            foreach (var row in tableRows.Skip(1))
            {
                var hoursAndMinutes = row.Descendants().ToList();
                // there's always 6 entries, but sometimes there can be four columns - so 8 entries
                // first HH, first mm - weekdays
                // second HH, second mm - winter holidays
                // third HH, third mm - saturday, sunday
                // fourth HH, fourth mm - holidays

                AddLineStopTimes(lineStopTimesToAdd, hoursAndMinutes[0], hoursAndMinutes[1], lineStopId, isWeekday: true);
                AddLineStopTimes(lineStopTimesToAdd, hoursAndMinutes[2], hoursAndMinutes[3], lineStopId, isWinterHoliday: true);
                if (hoursAndMinutes.Count == 6)
                {
                    AddLineStopTimes(lineStopTimesToAdd, hoursAndMinutes[4], hoursAndMinutes[5], lineStopId, isSaturdaySunday: true, isHolidays: true);
                }
                if (hoursAndMinutes.Count == 8)
                {
                    AddLineStopTimes(lineStopTimesToAdd, hoursAndMinutes[4], hoursAndMinutes[5], lineStopId, isSaturdaySunday: true);
                    AddLineStopTimes(lineStopTimesToAdd, hoursAndMinutes[6], hoursAndMinutes[7], lineStopId, isHolidays: true);
                }
            }

            return lineStopTimesToAdd;
        }

        public IEnumerable<LineStopUrl> ParseTimetablesUrls(string lineData)
        {
            var substring = Common.GetTextBetweenAndClean(lineData, "<div class=\"timetable-stops\">", "</table></div></div>", Common.XmlEscapeReplacements);

            var document = XDocument.Parse(substring);
            var urls = document.XPathSelectElements("div/div/table/tr/td/a").ToList();
            var times = document.XPathSelectElements("div/div/table/tr/td")
                .Where(t => t.Attributes().Any(a => a.Value == "czas")).ToList();

            foreach (var (url, time) in urls.Zip(times))
            {
                var lineStopUrl = url.Attributes().FirstOrDefault(a => a.Name == "href")?.Value ?? string.Empty;
                yield return new LineStopUrl { Url = lineStopUrl, StopName = url.Value };
            }
        }

        private static void AddLineStopTimes(
            List<LineStopTime> lineStopTimesToAdd,
            XElement hourCell,
            XElement minuteCell,
            int lineStopId,
            bool isWeekday = false,
            bool isWinterHoliday = false,
            bool isSaturdaySunday = false,
            bool isHolidays = false)
        {
            var arrivals = GetArrivalsDayMinutes(hourCell, minuteCell)
                    .Select(a => new LineStopTime
                    {
                        LineStopId = lineStopId,
                        DayMinute = a,
                        IsWeekday = isWeekday,
                        IsWinterHoliday = isWinterHoliday,
                        IsSaturdaySundays = isSaturdaySunday,
                        IsHolidays = isHolidays
                    });
            lineStopTimesToAdd.AddRange(arrivals);
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
