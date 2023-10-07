using ConsoleDemo.Interfaces;
using ConsoleDemo.Parsers;
using TorunLive.Domain.Entities;
using TorunLive.Domain.Enums;

namespace ConsoleDemo.Services
{
    public class TimetableService : ITimetableService
    {
        public async Task<Timetable> GetTimetable(int startStopId, PolishDayOfWeek dayOfWeek, int dayMinute)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("https://www.rozkladzik.pl/torun/timetable.txt");
            var requestArgs = $"?c=tsa&t={startStopId}&day={(int)dayOfWeek}&time={dayMinute}";
            var response = await client.GetAsync(requestArgs);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var parser = new TimetableParser();
            var parsed = parser.Parse(responseBody);
            return parsed;
        }
    }
}
