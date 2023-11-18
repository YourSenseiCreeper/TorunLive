using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using TorunLive.Application.Extensions;
using TorunLive.Application.Interfaces.Adapters;
using TorunLive.Application.Interfaces.Services;
using TorunLive.Domain.Database;
using TorunLive.Domain.Entities;
using TorunLive.Domain.Enums;
using TorunLive.Persistance;

namespace TorunLive.Application.Services
{
    public class FullTimetableService(
        TorunLiveContext dbContext,
        ITimetableComparatorService timetableComparator,
        ILiveRequestService liveRequestService,
        ILiveTimetableAdapter liveTimetableAdapter,
        IDateTimeService dateTimeService
    ) : IFullTimetableService
    {
        private readonly TorunLiveContext _dbContext = dbContext;
        private readonly ITimetableComparatorService _timetableComparator = timetableComparator;
        private readonly ILiveRequestService _liveRequestService = liveRequestService;
        private readonly ILiveTimetableAdapter _liveTimetableAdapter = liveTimetableAdapter;
        private readonly IDateTimeService _dateTimeService = dateTimeService;

        public async Task<IEnumerable<CompareLine>> GetFullTimetable(string sipStopId)
        {
            var stopId = int.Parse(sipStopId);
            var now = _dateTimeService.Now;
            var polishDayOfWeek = (PolishDayOfWeek)Enum.Parse(typeof(PolishDayOfWeek), now.DayOfWeek.ToString());
            bool isWeekday = now.DayOfWeek <= DayOfWeek.Friday;
            bool isSaturdaySunday = now.DayOfWeek >= DayOfWeek.Saturday;
            bool isHolidays = false;

            var lineStopsForStop = _dbContext.LineStops.Where(ls => ls.StopId == sipStopId).ToList();
            // lista lini
            var currentDayMinute = now.ToDayMinute();
            var timeTreshold = currentDayMinute + 60;
            foreach(var lineStop in lineStopsForStop)
            {
                var lineStopTimeForStops = _dbContext.LineStopTimes.Where(lst => 
                    lst.LineStopId == lineStop.Id && 
                    lst.DayMinute >= currentDayMinute && lst.DayMinute <= timeTreshold &&
                    lst.IsWeekday == isWeekday && lst.IsSaturdaySundays == isSaturdaySunday && lst.IsHolidays == isHolidays)
                    .ToList();
            }

            var liveTimetableResponse = await _liveRequestService.GetTimetable(sipStopId);
            var liveTimetable = _liveTimetableAdapter.Adapt(liveTimetableResponse);

            var result = _timetableComparator.Compare(new LiveTimetable(), liveTimetable); // todo: replace new object with timetable from db

            return result;
        }

        public async Task<CompareLine> GetLiveForLine(string lineNumber, string stopId, string direction)
        {
            // tymczasowo niech directionId będzie z palca
            var directionId = 1;
            var lastNStops = 5;
            //var directionId = _dbContext.Directions.Single(d => d.LineId == lineNumber && )
            var stopsBefore = _dbContext.LineStops.Where(ls =>
                ls.LineId == lineNumber &&
                ls.StopId == stopId &&
                ls.DirectionId == directionId)
                .OrderBy(ls => ls.StopOrder)
                .Take(lastNStops)
                .Include(ls => ls.Stop)
                //.Include(ls => ls.LineStopTimes.Where(lst => lst.DayMinute)) // todo: use lineStopTimes to tell if line is delayed
                .ToList();
            
            var startingStop = stopsBefore.Last();
            var stopName = startingStop.Stop.Name;
            var timeFromLineStart = startingStop.TimeToNextStop ?? 0; // todo: not working right now

            var arrivals = new ConcurrentBag<CompareArrival>();
            await Parallel.ForEachAsync(stopsBefore, async (stop, cancellationToken)
                => await ProcessStop(arrivals, stop));

            var result = new CompareLine
            {
                Name = lineNumber,
                Direction = direction,
                Arrivals = arrivals.OrderBy(x => x.Order).ToList()
            };

            return result;
        }

        private async Task ProcessStop(ConcurrentBag<CompareArrival> arrivals, LineStop stop)
        {
            var liveTimetableResponse = await _liveRequestService.GetTimetable(stop.StopId.ToString());
            var liveTimetable = _liveTimetableAdapter.Adapt(liveTimetableResponse);

            var liveLineEntries = liveTimetable.Lines.Where(l => l.Number == stop.LineId);
            if (!liveLineEntries.Any())
                return;

            var liveLineEntry = liveLineEntries.First();
            var diffTime = stop.TimeToNextStop ?? 0; // todo: not working right now;
                                                                         // pobieramy pierwsze, aby uniknąć pomylenia kolejnego przejazdu z przejazdem opóźnionym
            var arrivalDayMinute = liveLineEntry.ArrivalsInDayMinutes.FirstOrDefault();
            arrivals.Add(new CompareArrival
            {
                Order = stop.StopOrder,
                StopId = stop.StopId,
                BaseDayMinute = diffTime,
                StopName = stop.Stop.Name,
                LiveDayMinute = arrivalDayMinute
            });
        }
    }
}
