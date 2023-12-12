using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using System.Xml.XPath;
using TorunLive.Application.Interfaces.Adapters;
using TorunLive.Application.Interfaces.Services;
using TorunLive.Common;
using TorunLive.Domain.Entities;

namespace TorunLive.Application.Adapters
{
    public class LiveTimetableAdapter(
        ILogger<LiveTimetableAdapter> logger,
        IDateTimeService dateTimeService
    ) : ILiveTimetableAdapter
    {
        private readonly ILogger<LiveTimetableAdapter> _logger = logger;
        private readonly IDateTimeService _dateTimeService = dateTimeService;

        private static readonly Regex ArrivingRegex = new(">>");
        private static readonly Regex ArrivalInMinutesRegex = new("[0-9]{1,2}min");
        private static readonly Regex ArrivalAtRegex = new("[0-9]{1,2}:[0-9]{1,2}");

        public LiveTimetable Adapt(string data)
        {
            var substring = HtmlStringExctractor.GetTextBetweenAndClean(data, "<table class=\"tablePanel\"", "</table>");
            var document = XDocument.Parse(substring);
            var htmlArrivals = document.XPathSelectElements("table/tbody/tr");

            //todo: skanować nagłówki tabel, aby dobrze przydzielać czasy przyjazdów
            // czasami są tylko dwie kolumny!

            // possible two entries of the same line
            var lines = AdaptArrivals(htmlArrivals)
                .GroupBy(a => a.Number)
                .Select(a =>
                {
                    var arrival = a.First();
                    return new LiveLine
                    {
                        Name = arrival.Name,
                        Number = arrival.Number,
                        ArrivalsInDayMinutes = a.Select(la => la.DayMinute).ToList()
                    };
                }).ToList();

            return new LiveTimetable { Lines = lines };
        }

        private IEnumerable<LiveArrival> AdaptArrivals(IEnumerable<XElement> htmlArrivals)
        {
            foreach (var htmlArrival in htmlArrivals)
            {
                var cells = htmlArrival.XPathSelectElements("td").ToList();
                if (cells.Count != 3)
                {
                    _logger.LogWarning("Cell count is not 3! Found {count}", cells.Count);
                    continue;
                }

                var lineNumber = cells[0].Value.Trim();
                var lineName = cells[1].Value.Trim();
                var time = cells[2].Value.Trim();
                yield return new LiveArrival
                {
                    Number = lineNumber,
                    Name = lineName,
                    DayMinute = ConvertArrivalTimeToDayMinute(time)
                };
            }
        }

        private int ConvertArrivalTimeToDayMinute(string minutesOrHourMinute)
        {
            var now = _dateTimeService.Now;
            var hour = 0;
            var minute = 0;

            if (ArrivingRegex.Match(minutesOrHourMinute).Success)
            {
                hour = now.Hour;
                minute = now.Minute;
            }
            else if (ArrivalInMinutesRegex.Match(minutesOrHourMinute).Success)
            {
                var value = int.Parse(minutesOrHourMinute.Replace("min", ""));
                var arrivalAfterMinutes = now.AddMinutes(value);
                hour = arrivalAfterMinutes.Hour;
                minute = arrivalAfterMinutes.Minute;
            }
            else if (ArrivalAtRegex.Match(minutesOrHourMinute).Success)
            {
                var hourAndMinute = minutesOrHourMinute.Split(':');
                hour = int.Parse(hourAndMinute[0]);
                minute = int.Parse(hourAndMinute[1]);
            }
            else
            {
                throw new ArgumentException($"Input string '{minutesOrHourMinute}' not matching any time pattern");
            }

            var dayMinute = hour * 60 + minute;
            return dayMinute;
        }
    }
}
