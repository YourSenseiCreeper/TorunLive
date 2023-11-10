using Microsoft.Extensions.Logging;
using TorunLive.Domain.EntitiesV2;
using TorunLive.Persistance;
using TorunLive.SIPTimetableScanner.Interfaces;

namespace TorunLive.SIPTimetableScanner.Services
{
    public class TimetableService : ITimetableService
    {
        private readonly TorunLiveContext _dbContext;
        private readonly HttpClient _httpClient;
        private readonly ILogger _logger;
        private readonly ITimetableParserService _timetableParserService;
        private readonly IDelayService _delayService;
        private readonly IRequestService _requestService;

        public TimetableService(
            ILogger<TimetableService> logger,
            HttpClient httpClient,
            TorunLiveContext dbContext,
            ITimetableParserService timetableParserService,
            IDelayService delayService,
            IRequestService requestService)
        {
            _logger = logger;
            _httpClient = httpClient;
            _dbContext = dbContext;
            _timetableParserService = timetableParserService;
            _delayService = delayService;
            _requestService = requestService;
        }

        public async Task ScanLineDirectionStopAndStopTimes(Entities.LineDirection direction, List<string> lineUrls)
        {
            _logger.LogInformation("Found '{directionName}', scanning stops...", direction.DirectionName);
            var secondLineUrl = lineUrls.Except(new[] { direction.Url }).First();
            var timetableStopUrlsAndNames = await GetLineStopsUrls(direction.Url);
            int index = 0;

            var urlData = direction.Url.Split('/');
            var lineId = urlData[0];
            var directionId = int.Parse(urlData[1]);

            if (!_dbContext.Directions.Any(s => s.LineId == lineId && s.DirectionId == directionId))
            {
                _dbContext.Directions.Add(new Direction
                {
                    LineId = lineId,
                    DirectionId = directionId,
                    Name = direction.DirectionName
                });
            }

            foreach (var (stopUrl, stopName) in timetableStopUrlsAndNames)
            {
                var stopId = stopUrl.Replace(".html", "");
                if (!_dbContext.Stops.Any(s => s.Id == stopId))
                {
                    var fixedName = stopName.Replace("nż.", "");
                    _dbContext.Stops.Add(new Domain.EntitiesV2.Stop { Id = stopId, Name = fixedName });
                }

                var lineStop = _dbContext.LineStops.Add(new LineStop
                {
                    LineId = lineId,
                    DirectionId = directionId,
                    StopId = stopId,
                    StopOrder = index++,
                    IsOnDemand = stopName.Contains("nż.")
                    //TimeToNextStop = 1
                }).Entity;
                _dbContext.SaveChanges();

                var timetableUrl = $"{lineId}/{directionId}/{stopId}.html";
                var response = await _httpClient.GetAsync(timetableUrl);
                response.EnsureSuccessStatusCode();
                var rawData = await response.Content.ReadAsStringAsync();

                await _delayService.Delay();

                var lineStopTimes = _timetableParserService.ParseArrivals(lineStop.Id, rawData);
                _dbContext.LineStopTimes.AddRange(lineStopTimes);
                _dbContext.SaveChanges();
            }

            await _delayService.Delay();
        }

        public async Task<List<(string, string)>> GetLineStopsUrls(string targetLineUrl)
        {
            var htmlString = await _requestService.GetTimetable(targetLineUrl);
            var parsed = _timetableParserService.ParseTimetablesUrls(htmlString).ToList();
            var fixedFirstItem = (targetLineUrl.Split('/')[2], parsed[0].Item2);
            parsed[0] = fixedFirstItem;
            return parsed;
        }
    }
}
