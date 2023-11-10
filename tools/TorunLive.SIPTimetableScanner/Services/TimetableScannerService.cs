using Microsoft.Extensions.Logging;
using TorunLive.SIPTimetableScanner.Interfaces;

namespace TorunLive.SIPTimetableScanner.Services
{
    public class TimetableScannerService : ITimetableScannerService
    {
        private readonly ILogger _logger;
        private readonly ITimetableService _timetableService;
        private readonly ILineDirectionsService _lineDirectionsService;
        private readonly IDelayService _delayService;

        private static readonly string[] _lines = new[]
        {
            "1", "2", "3", "4", "5", "6", "7",
            "10", "11", "12", "13", "14", "15", "16", "17",
            "18", "19", "20", "24", "25", "26", "27", "28",
            "29", "30", "31", "32", "33", "34", "38", "39",
            "40", "41", "42", "44", "111", "112", "113", "115",
            "121", "122", "131", "N90", "N91", "N93", "N94", "N95"
        };

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

        public async Task ScanTimetablesAndLines()
        {
            try
            {
                foreach (var line in _lines)
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
