using Microsoft.Extensions.Logging;
using TorunLive.SIPTimetableScanner.Interfaces.Services;

namespace TorunLive.SIPTimetableScanner.Services
{
    public class TimetableScannerService : ITimetableScannerService
    {
        private readonly ILogger _logger;
        private readonly ITimetableService _timetableService;
        private readonly ILineDirectionsService _lineDirectionsService;
        private readonly IDelayService _delayService;

        public TimetableScannerService(
            ILogger<TimetableScannerService> logger,
            IDelayService delayService,
            ILineDirectionsService lineDirectionsService,
            ITimetableService timetableService
            )
        {
            _logger = logger;
            _delayService = delayService;
            _lineDirectionsService = lineDirectionsService;
            _timetableService = timetableService;
        }

        public async Task ScanTimetablesAndLines(string[] lines)
        {
            try
            {
                foreach (var line in lines)
                {
                    _logger.LogInformation("Scanning line {line}", line);
                    var directions = await _lineDirectionsService.GetLineDirections(line);
                    var lineUrls = directions.Select(d => d.Url).ToList();

                    foreach (var direction in directions)
                    {
                        await _timetableService.ScanLineDirectionStopAndStopTimes(direction, lineUrls);
                    }

                    await _delayService.Delay();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
