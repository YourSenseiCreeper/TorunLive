using System.Collections.Concurrent;
using TorunLive.Application.Extensions;
using TorunLive.Application.Interfaces.Services;
using TorunLive.Domain.Entities;
using TorunLive.Domain.Enums;

namespace TorunLive.Application.Services
{
    public class FullTimetableService : IFullTimetableService
    {
        private readonly ITimetableService _timetableService;
        private readonly ITimetableComparatorService _timetableComparator;
        private readonly ILiveTimetableService _liveTimetableService;
        private readonly ILineStopsService _lineStopsService;

        public FullTimetableService(
            ITimetableService timetableService,
            ITimetableComparatorService timetableComparator,
            ILiveTimetableService liveTimetableService,
            ILineStopsService lineStopsService
            )
        {
            _timetableService = timetableService;
            _timetableComparator = timetableComparator;
            _liveTimetableService = liveTimetableService;
            _lineStopsService = lineStopsService;
        }

        public async Task<IEnumerable<CompareLine>> GetFullTimetable(string sipStopId)
        {
            var stopId = int.Parse(sipStopId);
            var startStopId = StopIdsMap.SIPtoRozkladzik[stopId];
            var now = DateTime.Now;
            var polishDayOfWeek = (PolishDayOfWeek)Enum.Parse(typeof(PolishDayOfWeek), now.DayOfWeek.ToString());

            // todo: replace with database search
            var baseTimetable = await _timetableService.GetTimetable(startStopId, polishDayOfWeek, now.ToDayMinute());

            var liveTimetable = await _liveTimetableService.GetTimetable(stopId);
            var result = _timetableComparator.Compare(baseTimetable, liveTimetable);

            return result;
        }

        public async Task<CompareLine> GetLiveForLine(string lineNumber, int stopId, string direction)
        {
            var stopsBeforeForStop = _lineStopsService.GetEntriesBeforeStop(lineNumber, direction, stopId, 5);
            //stopsBeforeForStop.Reverse();
            for (int i = 0; i < stopsBeforeForStop.Count; i++)
                stopsBeforeForStop[i].Order = i;
            
            var startingStop = stopsBeforeForStop.Last();
            var stopName = startingStop.Name;
            var timeFromLineStart = startingStop.TimeElapsedFromFirstStop;

            var arrivals = new ConcurrentBag<CompareArrival>();
            await Parallel.ForEachAsync(stopsBeforeForStop, async (timetableStop, cancellationToken) =>
            {
                var liveTimetable = await _liveTimetableService.GetTimetable(timetableStop.StopId);
                var liveLineEntries = liveTimetable.Lines.Where(l => l.Number == lineNumber);
                if (liveLineEntries.Any())
                {
                    var liveLineEntry = liveLineEntries.First();
                    var diffTime = timeFromLineStart - timetableStop.TimeElapsedFromFirstStop;
                    // pobieramy pierwsze, aby uniknąć pomylenia kolejnego przejazdu z przejazdem opóźnionym
                    var arrival = liveLineEntry.Arrivals.FirstOrDefault();
                    arrivals.Add(new CompareArrival
                    {
                        Order = timetableStop.Order,
                        StopId = timetableStop.StopId,
                        BaseDayMinute = diffTime,
                        StopName = timetableStop.Name,
                        ActualBaseMinute = arrival?.DayMinute
                    });
                }
            });

            var result = new CompareLine
            {
                Name = lineNumber,
                Direction = direction,
                Arrivals = arrivals.OrderBy(x => x.Order).ToList()
            };

            return result;
        }
    }
}
