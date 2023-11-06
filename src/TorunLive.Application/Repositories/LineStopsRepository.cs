using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using TorunLive.Application.Interfaces.Repositories;
using TorunLive.Domain.Entities;

namespace TorunLive.Application.Repositories
{
    public class LineStopsRepository : ILineStopsRepository
    {
        private readonly List<LineEntry> _lineEntries;

        public LineStopsRepository(IConfiguration configuration)
        {
            var filename = configuration.GetRequiredSection(Constants.TimetablePath).Value ?? string.Empty;
            var path = Path.Combine(Directory.GetCurrentDirectory(), filename);
            var serialized = File.ReadAllText(path);
            _lineEntries = JsonConvert.DeserializeObject<List<LineEntry>>(serialized) ?? new List<LineEntry>();
        }

        public LineEntry? GetForLineAndDirection(string lineName, string lineDirection)
        {
            return _lineEntries.Where(l => l.Name == lineName)
                .FirstOrDefault(l => l.DirectionName == lineDirection);
        }
    }
}
