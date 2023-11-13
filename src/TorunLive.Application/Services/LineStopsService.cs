using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
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

        public List<TimetableEntry> GetEntriesBeforeStop(string lineName, string direction, int stopId, int amountStopsBefore)
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

        public Task<List<LineDirection>> GetLineDirectionsForStop(string stopId)
        {
            var validationRegex = new Regex("^[0-9]{5}$");
            var match = validationRegex.Match(stopId);
            if (!match.Success)
            {
                _logger.LogError($"Not valid stopId: '{stopId}'");
                throw new ArgumentException($"Not valid stopId: '{stopId}'");
            }
            return _lineStopsRepository.GetLineDirectionsForStop(stopId);
        }
    }
}
