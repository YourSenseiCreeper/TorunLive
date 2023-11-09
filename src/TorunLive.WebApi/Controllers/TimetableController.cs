using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TorunLive.Application.Interfaces.Services;
using TorunLive.Domain.Entities;
using TorunLive.Persistance;

namespace TorunLive.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class TimetableController : ControllerBase
    {
        private readonly ILogger<TimetableController> _logger;
        private readonly IFullTimetableService _fullTimetableService;
        private readonly ILineStopsService _lineStopsService;
        private readonly TorunLiveContext _dbContext;

        public TimetableController(
            ILogger<TimetableController> logger,
            IFullTimetableService fullTimetableService,
            ILineStopsService lineStopsService,
            TorunLiveContext dbContext)
        {
            _logger = logger;
            _fullTimetableService = fullTimetableService;
            _lineStopsService = lineStopsService;
            _dbContext = dbContext;
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

        [HttpGet]
        public async Task<List<Domain.EntitiesV2.Line>> GetLines()
        {
            return await _dbContext.Lines.ToListAsync();
        }
    }
}