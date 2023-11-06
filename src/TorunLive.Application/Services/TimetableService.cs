using Microsoft.Extensions.Configuration;
using TorunLive.Application.Interfaces.Parsers;
using TorunLive.Application.Interfaces.Services;
using TorunLive.Domain.Entities;
using TorunLive.Domain.Enums;

namespace TorunLive.Application.Services
{
    public class TimetableService : ITimetableService
    {
        private readonly string _rozkladzikTimetableUrl;
        private readonly ITimetableParser _parser;

        public TimetableService(
            IConfiguration configuration,
            ITimetableParser parser)
        {
            _rozkladzikTimetableUrl = configuration.GetRequiredSection(Constants.RozkladzikTimetableUrl).Value ?? string.Empty;
            _parser = parser;
        }

        public async Task<Timetable> GetTimetable(int startStopId, PolishDayOfWeek dayOfWeek, int dayMinute)
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(_rozkladzikTimetableUrl)
            };
            var requestArgs = $"?c=tsa&t={startStopId}&day={(int)dayOfWeek}&time={dayMinute}";
            var response = await client.GetAsync(requestArgs);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var parsedResponse = _parser.Parse(responseBody);
            return parsedResponse;
        }
    }
}
