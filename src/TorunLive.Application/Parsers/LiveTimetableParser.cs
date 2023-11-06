using System.Xml.Linq;
using System.Xml.XPath;
using TorunLive.Application.Interfaces.Parsers;
using TorunLive.Domain.Entities;

namespace TorunLive.Application.Parsers
{
    public class LiveTimetableParser : ILiveTimetableParser
    {
        public LiveTimetable Parse(string data)
        {
            //data = Example;
            //var panelName = "ctl00_ctl00_ContentPlaceHolderContenido_UpdatePanel1|";
            //var endMarker = "|0|hiddenField";
            var panelName = "<table class=\"tablePanel\"";
            var startIndex = data.IndexOf(panelName);
            var endMarker = "</table>";
            var endIndex = data.IndexOf(endMarker);
            var substring = data.Substring(startIndex, endIndex + endMarker.Length - startIndex);

            var document = XDocument.Parse(substring);
            var arrivals = document.XPathSelectElements("table/tbody/tr");
            var liveArrivals = new List<LiveArrival>();
            foreach (var arrival in arrivals)
            {
                var cells = arrival.XPathSelectElements("td").ToList();
                if (cells.Count != 3)
                {
                    Console.WriteLine("Cell count is not 3!");
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
