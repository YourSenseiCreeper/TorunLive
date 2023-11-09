using Microsoft.AspNetCore.Mvc;
using TorunLive.Application.Interfaces.Services;
using TorunLive.Domain.Entities;

namespace TorunLive.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class TimetableController : ControllerBase
    {
        private readonly ILogger<TimetableController> _logger;
        private readonly IFullTimetableService _fullTimetableService;
        private readonly ILineStopsService _lineStopsService;

        public TimetableController(
            ILogger<TimetableController> logger,
            IFullTimetableService fullTimetableService,
            ILineStopsService lineStopsService)
        {
            _logger = logger;
            _fullTimetableService = fullTimetableService;
            _lineStopsService = lineStopsService;
        }

        [HttpGet]
        public async Task<List<CompareLine>> GetTimetable(int sipStopId)
        {
            return await _fullTimetableService.GetFullTimetable(sipStopId);
        }

        [HttpGet]
        public async Task<CompareLine> GetDelay(string lineNumber, int sipStopId, string direction)
        {
            return await _fullTimetableService.GetLiveForLine(lineNumber, sipStopId, direction);
        }

        [HttpGet]
        public List<LineDirection> GetLineDirections(int sipStopId)
        {
            return _lineStopsService.GetLineDirectionsForStop(sipStopId);
        }
    }
}