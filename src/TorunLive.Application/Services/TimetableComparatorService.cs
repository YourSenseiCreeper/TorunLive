using TorunLive.Application.Interfaces.Services;
using TorunLive.Domain.Entities;

namespace TorunLive.Application.Services
{
    public class TimetableComparatorService : ITimetableComparatorService
    {
        public IEnumerable<CompareLine> Compare(Timetable baseTimetable, LiveTimetable liveTimetable)
        {
            foreach (var liveLine in liveTimetable.Lines)
            {
                var comparedLine = new CompareLine
                {
                    Name = liveLine.Name,
                    Number = liveLine.Number,
                    Arrivals = new List<CompareArrival>()
                };
                var baseLine = baseTimetable.Lines.SingleOrDefault(l => l.Number == liveLine.Number);
                if (baseLine == null)
                {
                    comparedLine.Error = $"ERR: Nie znaleziono lini '{liveLine.Name}' na liście linini z rozkładu";
                    continue;
                }

                var minEntries = Math.Min(liveLine.Arrivals.Count, baseLine.Arrivals.Count);
                for (int i = 0; i < minEntries; i++)
                {
                    var baseArrival = baseLine.Arrivals[i];
                    var liveArrival = liveLine.Arrivals[i];
                    var compareArrival = new CompareArrival
                    {
                        BaseDayMinute = baseArrival.DayMinute,
                        ActualBaseMinute = liveArrival.DayMinute,
                        Delay = baseArrival.DayMinute - liveArrival.DayMinute,
                        StopId = baseArrival.PossibleStopNumber
                    };
                    comparedLine.Arrivals.Add(compareArrival);
                }
                yield return comparedLine;
            }
        }
    }
}
