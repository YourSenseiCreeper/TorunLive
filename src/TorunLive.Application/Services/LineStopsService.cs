using Newtonsoft.Json;
using TorunLive.Application.Interfaces.Services;
using TorunLive.Domain.Entities;

namespace TorunLive.Application.Services
{
    public class LineStopsService : ILineStopsService
    {
        private List<LineEntry> _lines;
        public LineStopsService()
        {
            _lines = new List<LineEntry>();
        }

        public void LoadFromFile(string filename)
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var path = Path.Combine(currentDirectory, filename);
            var serialized = File.ReadAllText(path);
            _lines = JsonConvert.DeserializeObject<List<LineEntry>>(serialized) ?? new List<LineEntry>();
        }

        public List<TimetableEntry> GetEntriesBeforeStop(string lineName, string direction, string stopId, int amountStopsBefore)
        {
            var lineWithTimetables = _lines.Where(l => l.Name == lineName);
            var line = lineWithTimetables.FirstOrDefault(l => l.DirectionName == direction);
            var stopInLine = line.Timetable.FindIndex(0, s => s.StopId == stopId);
            stopInLine -= Math.Min(stopInLine, amountStopsBefore) - 1;

            var stopsToScan = line.Timetable.Skip(stopInLine).Take(amountStopsBefore).ToList();
            return stopsToScan;
        }
    }
}
