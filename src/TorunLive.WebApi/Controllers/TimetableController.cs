using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TorunLive.Application.Interfaces.Services;
using TorunLive.Persistance;

namespace TorunLive.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]/[action]")]
    public class TimetableController(
        ILogger<TimetableController> logger,
        IFullTimetableService fullTimetableService,
        TorunLiveContext dbContext) : ControllerBase
    {
        private readonly ILogger<TimetableController> _logger = logger;
        private readonly IFullTimetableService _fullTimetableService = fullTimetableService;
        private readonly TorunLiveContext _dbContext = dbContext;

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetTimetable(string sipStopId)
        {
            return new OkObjectResult(await _fullTimetableService.GetFullTimetable(sipStopId));
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetDelay(string lineNumber, string sipStopId, int directionId)
        {
            var result = await _fullTimetableService.GetLiveForLine(lineNumber, sipStopId, directionId);
            if (result.IsSuccess)
                return new OkObjectResult(result.ResultObject);

            _logger.LogError(result.ErrorMessage);
            return new BadRequestObjectResult(result.ErrorMessage);
        }

        [HttpGet]
        public Task<IEnumerable<DateTime>> GetNextArrivals(string lineNumber, int directionId, string stopId)
        {
            return _fullTimetableService.GetNextArrivals(lineNumber, directionId, stopId);
        }

        [HttpGet]
        public Task<List<Domain.Database.Line>> GetLines()
        {
            return _dbContext.Lines.Include(l => l.Directions).ToListAsync();
        }

        [HttpGet]
        public async Task<List<Domain.Database.LineStop>> GetLineStops(string lineNumber, int directionId)
        {
            var lineStops = await _dbContext.LineStops.Where(ls =>
                ls.LineId == lineNumber &&
                ls.DirectionId == directionId)
                .Include(ls => ls.Stop)
                .OrderBy(ls => ls.StopOrder)
                .ToListAsync();

            return lineStops;
        }
    }
}