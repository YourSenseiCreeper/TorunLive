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
            //var liveLinesThatExistInBaseTimetable = liveTimetable.Lines.Where(lt => baseTimetable.Lines.Any(l => l.Number == lt.Number));
            //var liveLinesThatExistInBaseTimetable = liveTimetable.Lines
            //    .Join(baseTimetable.Lines,
            //        live => live.Number,
            //        baseLine => baseLine.Number,
            //        (live, baseLine) => live);

            //var missingBaseLines = baseTimetable.Lines.Except(liveLinesThatExistInBaseTimetable)
            //    .Select(l => new CompareLine
            //    {
            //        Name = l.Name,
            //        Number = l.Number,
            //        Error = $"ERR: Nie znaleziono lini na liście linini z rozkładu"
            //    });
            //foreach (var missingLine in missingBaseLines)
            //    yield return missingLine;

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
                        LiveDayMinute = liveArrivalDayMinute,
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
