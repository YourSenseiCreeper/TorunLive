using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using TorunLive.Application.Extensions;
using TorunLive.Application.Interfaces.Adapters;
using TorunLive.Application.Interfaces.Services;
using TorunLive.Common;
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
            var now = _dateTimeService.Now;
            var polishDayOfWeek = (PolishDayOfWeek)Enum.Parse(typeof(PolishDayOfWeek), now.DayOfWeek.ToString());
            bool isWeekday = now.DayOfWeek <= DayOfWeek.Friday;
            bool isSaturdaySunday = now.DayOfWeek >= DayOfWeek.Saturday;
            bool isHolidays = false;

            var stopId = int.Parse(sipStopId);
            var lineStopsForStop = _dbContext.LineStops.Where(ls => ls.StopId == sipStopId).ToList();
            // lista lini
            var currentDayMinute = now.ToDayMinute();
            var offset = currentDayMinute + 60;
            foreach(var lineStop in lineStopsForStop)
            {
                var lineStopTimeForStops = _dbContext.LineStopTimes.Where(lst => 
                    lst.LineStopId == lineStop.Id && 
                    lst.DayMinute >= currentDayMinute && lst.DayMinute <= offset &&
                    lst.IsWeekday == isWeekday && lst.IsSaturdaySundays == isSaturdaySunday && lst.IsHolidays == isHolidays)
                    .ToList();
            }

            var liveTimetableResponse = await _liveRequestService.GetTimetable(sipStopId);
            var liveTimetable = _liveTimetableAdapter.Adapt(liveTimetableResponse);

            var result = _timetableComparator.Compare(new LiveTimetable(), liveTimetable); // todo: replace new object with timetable from db

            return result;
        }

        public async Task<Result<CompareLine>> GetLiveForLine(string lineNumber, string stopId, int directionId)
        {
            var directStop = await _dbContext.LineStops.FirstOrDefaultAsync(ls =>
                ls.LineId == lineNumber &&
                ls.StopId == stopId &&
                ls.DirectionId == directionId);

            if (directStop == null)
                return Result<CompareLine>.Failure($"Not found stop");

            var lastNStops = 5;
            var now = _dateTimeService.Now;
            var nowDayMinute = now.ToDayMinute();
            var offset = nowDayMinute + 60;
            var stopsBefore = _dbContext.LineStops.Where(ls =>
                ls.LineId == lineNumber &&
                ls.DirectionId == directionId)
                .Where(ls => ls.StopOrder >= directStop.StopOrder - lastNStops && ls.StopOrder <= directStop.StopOrder)
                .OrderBy(ls => ls.StopOrder)
                .Include(ls => ls.Stop)
                .Include(ls => ls.Direction)
                .Include(ls => ls.LineStopTimes.Where(lst => lst.DayMinute >= nowDayMinute && lst.DayMinute <= offset))
                .ToList();
            
            //var arrivals = new ConcurrentBag<CompareArrival>();
            //foreach (var stop in stopsBefore)
            //{
            //    await CompareStop(arrivals, stop);
            //}

            var arrivals = new ConcurrentBag<CompareArrival>();
            await Parallel.ForEachAsync(stopsBefore, async (stop, cancellationToken)
                => await CompareStop(arrivals, stop));

            var result = new CompareLine
            {
                Name = lineNumber,
                Direction = directStop.Direction.Name,
                Arrivals = arrivals.OrderBy(x => x.Order).ToList()
            };

            return Result<CompareLine>.Success(result);
        }

        public async Task<IEnumerable<DateTime>> GetNextArrivals(string lineNumber, int directionId, string stopId)
        {
            var now = _dateTimeService.Now;
            var nowDayMinute = now.ToDayMinute();
            var offset = nowDayMinute + 60;
            var lineStopWithArrivals = await _dbContext.LineStops
                .Include(ls => ls.LineStopTimes)
                .FirstOrDefaultAsync(ls => 
                    ls.LineId == lineNumber &&
                    ls.DirectionId == directionId &&
                    ls.StopId == stopId);

            if (lineStopWithArrivals == null)
                return Array.Empty<DateTime>();

            var arrivals = lineStopWithArrivals.LineStopTimes
                .Where(lst => lst.DayMinute >= nowDayMinute && lst.DayMinute <= offset)
                .Select(lst => lst.DayMinute.GetDateTimeFromDayMinute(now.DateTime));

            return arrivals;
        }

        private async Task CompareStop(ConcurrentBag<CompareArrival> arrivals, LineStop stop)
        {
            var liveTimetableResponse = await _liveRequestService.GetTimetable(stop.StopId.ToString());
            var liveTimetable = _liveTimetableAdapter.Adapt(liveTimetableResponse);

            var liveLineEntries = liveTimetable.Lines.Where(l => l.Number == stop.LineId);
            if (!liveLineEntries.Any())
                return;

            // pobieramy pierwsze, aby uniknąć pomylenia kolejnego przejazdu z przejazdem opóźnionym
            var liveLineEntry = liveLineEntries.First();

            var arrivalDayMinute = liveLineEntry.ArrivalsInDayMinutes.FirstOrDefault();
            var closestMatchToBase = stop.LineStopTimes.MinBy(lst => Math.Abs(lst.DayMinute - arrivalDayMinute));
            arrivals.Add(new CompareArrival
            {
                Order = stop.StopOrder,
                StopId = stop.StopId,
                BaseDayMinute = closestMatchToBase.DayMinute,
                Delay = closestMatchToBase.DayMinute - arrivalDayMinute,
                StopName = stop.Stop.Name,
                LiveDayMinute = arrivalDayMinute
            });
        }
    }
}
