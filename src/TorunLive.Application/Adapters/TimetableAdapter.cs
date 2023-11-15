using System.Text.RegularExpressions;
using TorunLive.Application.Interfaces.Adapters;
using TorunLive.Domain.Entities;

namespace TorunLive.Application.Adapters
{
    public class TimetableAdapter : ITimetableAdapter
    {
        public Timetable Adapt(string data)
        {
            var parsedLines = new List<Line>();
            var lineDataAndArrivalsRegex = new Regex(@"(\d+);(\d+);(\d+)");
            var messageRegex = new Regex(@"\#\$\#([SR])\#\$\#(.+$)");
            var courses = data.Split(Constants.LINE_SEPARATOR);
            foreach (var course in courses)
            {
                var matches = lineDataAndArrivalsRegex.Matches(course);
                if (matches.Count == 0)
                    continue;

                var lineNumber = matches[0].Groups[1].Value;
                var unknownOne = int.Parse(matches[0].Groups[2].Value);
                var unknownTwo = int.Parse(matches[0].Groups[3].Value);

                var arrivalsParsed = new List<Arrival>();
                foreach (var arrival in matches.Skip(1))
                {
                    var dayTime = int.Parse(arrival.Groups[1].Value);
                    var arrivalUnknownOne = int.Parse(arrival.Groups[2].Value);
                    var possibleStopNumber = int.Parse(arrival.Groups[3].Value);
                    arrivalsParsed.Add(new Arrival
                    {
                        DayMinute = dayTime,
                        UnknownOne = arrivalUnknownOne,
                        PossibleStopNumber = possibleStopNumber
                    });
                }

                var messageMatch = messageRegex.Match(course);
                string? message = null;
                if (messageMatch.Success)
                {
                    var symbol = messageMatch.Groups[1].Value; // S - skrócona trasa, R - (rozszerzona?) dłuższa trasa niż zazwyczaj
                    var text = messageMatch.Groups[2].Value;
                    message = $"[{symbol}] {text}";
                }

                parsedLines.Add(new Line
                {
                    Number = lineNumber,
                    UnknownOne = unknownOne,
                    UnknownTwo = unknownTwo,
                    Arrivals = arrivalsParsed,
                    Message = message
                });
            }
            return new Timetable { Lines = parsedLines };
        }
    }

    static class Constants
    {
        public const string LINE_SEPARATOR = "|";
    }
}
