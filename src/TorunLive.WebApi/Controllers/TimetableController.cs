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
        private readonly TorunLiveContext _dbContext;

        public TimetableController(
            ILogger<TimetableController> logger,
            IFullTimetableService fullTimetableService,
            TorunLiveContext dbContext)
        {
            _logger = logger;
            _fullTimetableService = fullTimetableService;
            _dbContext = dbContext;
        }

        [HttpGet]
        public async Task<ActionResult> GetTimetable(string sipStopId)
        {
            return new OkObjectResult(await _fullTimetableService.GetFullTimetable(sipStopId));
        }

        [HttpGet]
        public async Task<CompareLine> GetDelay(string lineNumber, string sipStopId, string direction)
        {
            return await _fullTimetableService.GetLiveForLine(lineNumber, sipStopId, direction);
        }

        //[HttpGet]
        //public async Task<List<LineDirection>> GetLineDirections(string sipStopId)
        //{
        //    return await _lineStopsService.GetLineDirectionsForStop(sipStopId);
        //}

        [HttpGet]
        public async Task<List<Domain.EntitiesV2.Line>> GetLines()
        {
            return await _dbContext.Lines.ToListAsync();
        }
    }
}