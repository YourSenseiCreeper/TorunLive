using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TorunLive.Application.Interfaces.Services;
using TorunLive.Domain.Entities;
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
        public async Task<ActionResult> GetTimetable(string sipStopId)
        {
            return new OkObjectResult(await _fullTimetableService.GetFullTimetable(sipStopId));
        }

        [HttpGet]
        public Task<CompareLine> GetDelay(string lineNumber, string sipStopId, int directionId)
        {
            return _fullTimetableService.GetLiveForLine(lineNumber, sipStopId, directionId);
        }

        //[HttpGet]
        //public async Task<List<LineDirection>> GetLineDirections(string sipStopId)
        //{
        //    return await _lineStopsService.GetLineDirectionsForStop(sipStopId);
        //}

        [HttpGet]
        public Task<IEnumerable<DateTime>> GetNextArrivals(string lineNumber, int directionId, string stopId)
        {
            return _fullTimetableService.GetNextArrivals(lineNumber, directionId, stopId);
        }

        [HttpGet]
        public Task<List<Domain.Database.Line>> GetLines()
        {
            return _dbContext.Lines.ToListAsync();
        }
    }
}