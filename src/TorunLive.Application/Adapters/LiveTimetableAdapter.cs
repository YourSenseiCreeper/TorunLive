using Microsoft.Extensions.Logging;
using System.Xml.Linq;
using System.Xml.XPath;
using TorunLive.Application.Interfaces.Adapters;
using TorunLive.Common;
using TorunLive.Domain.Entities;

namespace TorunLive.Application.Adapters
{
    public class LiveTimetableAdapter : ILiveTimetableAdapter
    {
        private readonly ILogger<LiveTimetableAdapter> _logger;

        public LiveTimetableAdapter(ILogger<LiveTimetableAdapter> logger)
        {
            _logger = logger;
        }

        public LiveTimetable Adapt(string data)
        {
            var substring = HtmlStringExctractor.GetTextBetweenAndClean(data, "<table class=\"tablePanel\"", "</table>");
            var document = XDocument.Parse(substring);
            var arrivals = document.XPathSelectElements("table/tbody/tr");
            var liveArrivals = new List<LiveArrival>();
            foreach (var arrival in arrivals)
            {
                var cells = arrival.XPathSelectElements("td").ToList();
                if (cells.Count != 3)
                {
                    _logger.LogWarning("Cell count is not 3! Found {count}", cells.Count);
                    continue;
                }

                var lineNumber = cells[0].Value.Trim();
                var lineName = cells[1].Value.Trim();
                var time = cells[2].Value.Trim();
                liveArrivals.Add(new LiveArrival
                {
                    Number = lineNumber,
                    Name = lineName,
                    DayMinute = ConvertArrivalTimeToDayMinute(time)
                });
            }

            // possible two entries of the same line
            var lines = liveArrivals.GroupBy(k => k.Number)
                .Select(k =>
                {
                    var arrival = k.First();
                    return new Line
                    {
                        Name = arrival.Name,
                        Number = arrival.Number,
                        Arrivals = k.Select(la => new Arrival { DayMinute = la.DayMinute }).ToList()
                    };
                }).ToList();

            return new LiveTimetable { Lines = lines };
        }

        private static int ConvertArrivalTimeToDayMinute(string minutesOrHourMinute)
        {
            if (minutesOrHourMinute == ">>")
            {
                var now = DateTime.Now;
                var dayMinute = now.Hour * 60 + now.Minute;
                return dayMinute;
            }

            if (minutesOrHourMinute.Contains("min"))
            {
                var value = int.Parse(minutesOrHourMinute.Replace("min", ""));
                var arrivalDateTime = DateTime.Now.AddMinutes(value);
                var dayMinute = arrivalDateTime.Hour * 60 + arrivalDateTime.Minute;
                return dayMinute;
            }
            else
            {
                var hourAndMinute = minutesOrHourMinute.Split(':');
                var hour = int.Parse(hourAndMinute[0]);
                var minute = int.Parse(hourAndMinute[1]);
                var dayMinute = hour * 60 + minute;
                return dayMinute;
            }
        }
    }
}
