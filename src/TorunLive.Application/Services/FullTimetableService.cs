using TorunLive.Application.Extensions;
using TorunLive.Application.Interfaces.Services;
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

        public async Task GetFullTimetable(int sipStopId)
        {
            var startStopId = StopIdsMap.SIPtoRozkladzik[sipStopId];
            var now = DateTime.Now;
            var polishDayOfWeek = (PolishDayOfWeek)Enum.Parse(typeof(PolishDayOfWeek), now.DayOfWeek.ToString());

            var baseTimetable = await _timetableService.GetTimetable(startStopId, polishDayOfWeek, now.ToDayMinute());

            var liveTimetable = await _liveTimetableService.GetTimetable(sipStopId);
            var result = _timetableComparator.Compare(baseTimetable, liveTimetable);
            foreach (var comparedLine in result)
            {
                Console.WriteLine($"Linia: {comparedLine.Number} - {comparedLine.Name}");
                foreach (var comparedArrival in comparedLine.Arrivals)
                {
                    var basic = comparedArrival.BaseDayMinute.GetDateTimeFromDayMinute();
                    var actual = comparedArrival.ActualBaseMinute.GetDateTimeFromDayMinute();
                    Console.WriteLine($"Planowy: {basic:hh:mm}, Aktualny: {actual:hh:mm}");
                }
                Console.WriteLine("-------------");
            }
        }

        public async Task GetLiveForLine(string lineNumber, string stopId, string direction)
        {
            var stopsBeforeForStop = _lineStopsService.GetEntriesBeforeStop(lineNumber, direction, stopId, 5);

            var startingStop = stopsBeforeForStop.Last();
            var stopName = startingStop.Name;
            var timeFromLineStart = startingStop.TimeElapsedFromFirstStop;

            foreach (var timetableStop in stopsBeforeForStop)
            {
                var sipStopId = int.Parse(timetableStop.StopId);
                var liveTimetable = await _liveTimetableService.GetTimetable(sipStopId);
                var liveLineEntries = liveTimetable.Lines.Where(l => l.Number == lineNumber);
                if (liveLineEntries.Any())
                {
                    var liveLineEntry = liveLineEntries.First();
                    var diffTime = timeFromLineStart - timetableStop.TimeElapsedFromFirstStop;
                    var arrival = liveLineEntry.Arrivals.FirstOrDefault();
                    if (arrival != null)
                    {
                        // muszę odnosić się do rozkładu lini, nie do różnic czasów - niewykonalne będzie pokazać jakie jest opóźnienie dla danego przystanku
                        Console.WriteLine($"{timetableStop.Name}: Planowany: {diffTime}m, Aktualny: {arrival.DayMinute.GetDateTimeFromDayMinute()}");
                    }
                    else
                    {
                        Console.WriteLine($"{timetableStop.Name}: Planowany: {diffTime}m, Aktualny: brak");
                    }
                }
            }
        }
    }
}
