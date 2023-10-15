using Microsoft.AspNetCore.Mvc;
using TorunLive.Application.Interfaces.Services;

namespace TorunLive.WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TimetableController : ControllerBase
    {
        private readonly ILogger<TimetableController> _logger;
        private readonly IFullTimetableService _fullTimetableService;

        public TimetableController(ILogger<TimetableController> logger, IFullTimetableService fullTimetableService)
        {
            _logger = logger;
            _fullTimetableService = fullTimetableService;
        }

        [HttpGet(Name = "GetDelays")]
        public async Task<IEnumerable<string>> GetDelays(int sipStopId)
        {
            await _fullTimetableService.GetFullTimetable(sipStopId);
            return Array.Empty<string>();
        }
    }
}