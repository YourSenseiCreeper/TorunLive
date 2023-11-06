using Microsoft.Extensions.Logging;
using TorunLive.Application.Interfaces.Repositories;
using TorunLive.Application.Interfaces.Services;
using TorunLive.Domain.Entities;

namespace TorunLive.Application.Services
{
    public class LineStopsService : ILineStopsService
    {
        private readonly ILineStopsRepository _lineStopsRepository;
        private readonly ILogger _logger;

        public LineStopsService(
            ILogger<LineStopsService> logger,
            ILineStopsRepository lineStopsRepository)
        {
            _logger = logger;
            _lineStopsRepository = lineStopsRepository;
        }

        public List<TimetableEntry> GetEntriesBeforeStop(string lineName, string direction, string stopId, int amountStopsBefore)
        {
            var line = _lineStopsRepository.GetForLineAndDirection(lineName, direction);
            if (line == null)
            {
                _logger.LogError("Not found line stops for line {line} and direction {direction}", lineName, direction);
                return new List<TimetableEntry>();
            }

            var stopInLine = line.Timetable.FindIndex(0, s => s.StopId == stopId);
            stopInLine -= Math.Min(stopInLine, amountStopsBefore) - 1;
            var selectedStops = line.Timetable.Skip(stopInLine).Take(amountStopsBefore).ToList();
            return selectedStops;
        }
    }
}
