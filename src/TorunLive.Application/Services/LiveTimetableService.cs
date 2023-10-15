using TorunLive.Application.Interfaces.Services;
using TorunLive.Application.Parsers;
using TorunLive.Domain.Entities;

namespace TorunLive.Application.Services
{
    public class LiveTimetableService : ILiveTimetableService
    {
        public async Task<LiveTimetable> GetTimetable(int sipStopId)
        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://sip.um.torun.pl:8080/panels/0/default.aspx");
            var requestArgs = $"?stop={sipStopId}";
            var response = await client.GetAsync(requestArgs);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var parser = new LiveTimetableParser();
            var parsed = parser.Parse(responseBody);
            return parsed;
        }
    }
}
