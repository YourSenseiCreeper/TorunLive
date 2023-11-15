using Microsoft.Extensions.Logging;
using TorunLive.Domain.EntitiesV2;
using TorunLive.Persistance;
using TorunLive.SIPTimetableScanner.Entities;
using TorunLive.SIPTimetableScanner.Interfaces.Adapters;
using TorunLive.SIPTimetableScanner.Interfaces.Services;

namespace TorunLive.SIPTimetableScanner.Services
{
    public class TimetableService : ITimetableService
    {
        private readonly TorunLiveContext _dbContext;
        private readonly ILogger _logger;
        private readonly ITimetableAdapterService _timetableAdapterService;
        private readonly IDelayService _delayService;
        private readonly IRequestService _requestService;

        public TimetableService(
            ILogger<TimetableService> logger,
            TorunLiveContext dbContext,
            ITimetableAdapterService timetableAdapterService,
            IDelayService delayService,
            IRequestService requestService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _timetableAdapterService = timetableAdapterService;
            _delayService = delayService;
            _requestService = requestService;
        }

        public async Task ScanLineDirectionStopAndStopTimes(Entities.LineDirection direction, List<string> lineUrls)
        {
            _logger.LogInformation("Found '{directionName}', scanning stops...", direction.DirectionName);
            var lineStopNamesAndUrls = await GetLineStopsUrls(direction.Url);
            int index = 0;

            var urlData = direction.Url.Split('/');
            var lineId = urlData[0];
            var directionId = int.Parse(urlData[1]);

            AddLineIfNotExists(lineId);
            AddDirectionIfNotExists(lineId, directionId, direction.DirectionName);

            foreach (var stopNameAndUrl in lineStopNamesAndUrls)
            {
                var stopId = stopNameAndUrl.Url.Replace(".html", "");
                AddStopIfNotExists(stopId, stopNameAndUrl.StopName);

                var lineStop = _dbContext.LineStops.Add(new LineStop
                {
                    LineId = lineId,
                    DirectionLineId = lineId,
                    DirectionId = directionId,
                    StopId = stopId,
                    StopOrder = index++,
                    IsOnDemand = stopNameAndUrl.StopName.Contains("nż.")
                    //TimeToNextStop = 1
                }).Entity;
                _dbContext.SaveChanges();

                var httpString = await _requestService.GetTimetable(lineId, directionId, stopId);
                await _delayService.Delay();

                var lineStopTimes = _timetableAdapterService.ParseArrivals(lineStop.Id, httpString);
                _dbContext.LineStopTimes.AddRange(lineStopTimes);
                _dbContext.SaveChanges();
            }

            await _delayService.Delay();
        }

        private async Task<List<LineStopUrl>> GetLineStopsUrls(string targetLineUrl)
        {
            var htmlString = await _requestService.GetTimetable(targetLineUrl);
            var parsed = _timetableAdapterService.ParseTimetablesUrls(htmlString).ToList();
            // first stop doesn't have url
            parsed[0].Url = targetLineUrl.Split('/')[2];
            return parsed;
        }


        private void AddLineIfNotExists(string lineId)
        {
            if (_dbContext.Lines.Any(s => s.Id == lineId))
                return;

            _dbContext.Lines.Add(new Line
            {
                Id = lineId,
            });
        }

        private void AddDirectionIfNotExists(string lineId, int directionId, string directionName)
        {
            if (_dbContext.Directions.Any(s => s.LineId == lineId && s.DirectionId == directionId))
                return;

            _dbContext.Directions.Add(new Direction
            {
                LineId = lineId,
                DirectionId = directionId,
                Name = directionName
            });
        }

        private void AddStopIfNotExists(string stopId, string stopName)
        {
            if(_dbContext.Stops.Any(s => s.Id == stopId))
                return;

            var fixedName = stopName.Replace("nż.", "");
            _dbContext.Stops.Add(new Stop
            {
                Id = stopId,
                Name = fixedName
            });
        }
    }
}
