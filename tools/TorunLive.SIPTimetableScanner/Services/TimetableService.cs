using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;
using TorunLive.Domain.Database;
using TorunLive.Persistance;
using TorunLive.SIPTimetableScanner.Entities;
using TorunLive.SIPTimetableScanner.Interfaces.Adapters;
using TorunLive.SIPTimetableScanner.Interfaces.Services;

namespace TorunLive.SIPTimetableScanner.Services
{
    public class TimetableService(
        ILogger<TimetableService> logger,
        TorunLiveContext dbContext,
        ITimetableAdapterService timetableAdapterService,
        IDelayService delayService,
        IRequestService requestService) : ITimetableService
    {
        private readonly TorunLiveContext _dbContext = dbContext;
        private readonly ILogger _logger = logger;
        private readonly ITimetableAdapterService _timetableAdapterService = timetableAdapterService;
        private readonly IDelayService _delayService = delayService;
        private readonly IRequestService _requestService = requestService;

        public async Task ScanLineDirectionStopAndStopTimes(LineDirection direction, List<string> lineUrls)
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

                var existingLineStop = await _dbContext.LineStops.FirstOrDefaultAsync(s => s.LineId == lineId && s.DirectionId == directionId && s.StopId == stopId);
                if (existingLineStop == null)
                {
                    existingLineStop = _dbContext.LineStops.Add(new LineStop
                    {
                        LineId = lineId,
                        DirectionLineId = lineId,
                        DirectionId = directionId,
                        StopId = stopId,
                        StopOrder = index++,
                        IsOnDemand = stopNameAndUrl.StopName.Contains("nż.")
                    }).Entity;
                    _dbContext.SaveChanges();
                }

                var httpString = await _requestService.GetTimetable(lineId, directionId, stopId);
                var stopTimesBytes = Encoding.UTF8.GetBytes(httpString);
                var hash = Convert.ToHexString(SHA256.HashData(stopTimesBytes));

                if (existingLineStop.TimetableVersionHash == hash)
                    continue;

                var lineStopTimes = _timetableAdapterService.ParseArrivals(existingLineStop.Id, httpString);
                _dbContext.LineStopTimes.AddRange(lineStopTimes);
                existingLineStop.TimetableVersionHash = hash;
                _dbContext.SaveChanges();
                await _delayService.Delay();
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
