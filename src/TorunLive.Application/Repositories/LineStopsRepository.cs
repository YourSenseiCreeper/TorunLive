using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using TorunLive.Application.Interfaces.Repositories;
using TorunLive.Domain.Entities;
using TorunLive.Persistance;

namespace TorunLive.Application.Repositories
{
    public class LineStopsRepository : ILineStopsRepository
    {
        private readonly List<LineEntry> _lineEntries;
        private readonly TorunLiveContext _dbContext;
        public LineStopsRepository(TorunLiveContext dbContext)
        {
            _dbContext = dbContext;
            //var filename = configuration.GetRequiredSection(Constants.TimetablePath).Value ?? string.Empty;
            //var path = Path.Combine(Directory.GetCurrentDirectory(), filename);
            //var serialized = File.ReadAllText(path);
            //_lineEntries = JsonConvert.DeserializeObject<List<LineEntry>>(serialized) ?? new List<LineEntry>();
        }

        public LineEntry? GetForLineAndDirection(string lineName, string lineDirection)
        {
            var lines = _lineEntries.Where(l => l.Name == lineName);
            return lines.FirstOrDefault(l => l.DirectionName == lineDirection);
        }

        public async Task<List<LineDirection>> GetLineDirectionsForStop(string stopId)
        {
            var targetStop = await _dbContext.Stops
                .Include(s => s.LineStops).ThenInclude(ls => ls.Direction)
                .FirstOrDefaultAsync(s => s.Id == stopId);

            var directions = targetStop.LineStops.Select(ls => ls.Direction).ToList();

            return _lineEntries
                .Where(l => l.TimetableUrl.Contains(stopId.ToString()))
                .GroupBy(l => l.Name)
                .Select(le => new LineDirection
                {
                    LineName = le.Key,
                    Directions = le.Select(x => x.DirectionName).ToList()
                }).ToList();
        }
    }
}
