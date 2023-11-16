using TorunLive.Application.Interfaces.Services;
using TorunLive.Domain.Entities;

namespace TorunLive.Application.Services
{
    public class TimetableComparatorService : ITimetableComparatorService
    {
        public TimetableComparatorService()
        {
        }

        public IEnumerable<CompareLine> Compare(LiveTimetable baseTimetable, LiveTimetable liveTimetable)
        {
            foreach (var liveLine in liveTimetable.Lines)
            {
                var comparedLine = new CompareLine
                {
                    Name = liveLine.Name,
                    Number = liveLine.Number,
                    Arrivals = []
                };
                var baseLine = baseTimetable.Lines.SingleOrDefault(l => l.Number == liveLine.Number);
                if (baseLine == null)
                {
                    comparedLine.Error = $"ERR: Nie znaleziono lini '{liveLine.Name}' na liście linini z rozkładu";
                    continue;
                }

                var minEntries = Math.Min(liveLine.ArrivalsInDayMinutes.Count, baseLine.ArrivalsInDayMinutes.Count);
                for (int i = 0; i < minEntries; i++)
                {
                    var baseArrivalDayMinute = baseLine.ArrivalsInDayMinutes[i];
                    var liveArrivalDayMinute = liveLine.ArrivalsInDayMinutes[i];
                    var compareArrival = new CompareArrival
                    {
                        BaseDayMinute = baseArrivalDayMinute,
                        ActualBaseMinute = liveArrivalDayMinute,
                        Delay = baseArrivalDayMinute - liveArrivalDayMinute,
                        //StopId = baseArrival.PossibleStopNumber.ToString()
                    };
                    comparedLine.Arrivals.Add(compareArrival);
                }
                yield return comparedLine;
            }
        }
    }
}
