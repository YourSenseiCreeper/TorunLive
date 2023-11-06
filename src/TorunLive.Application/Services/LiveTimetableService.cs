using Microsoft.Extensions.Configuration;
using TorunLive.Application.Interfaces.Parsers;
using TorunLive.Application.Interfaces.Services;
using TorunLive.Domain.Entities;

namespace TorunLive.Application.Services
{
    public class LiveTimetableService : ILiveTimetableService
    {
        private readonly string _sipTimetableUrl;
        private readonly ILiveTimetableParser _parser;

        public LiveTimetableService(
            IConfiguration configuration,
            ILiveTimetableParser parser)
        {
            _sipTimetableUrl = configuration.GetRequiredSection(Constants.SipTimetableUrl).Value ?? string.Empty;
            _parser = parser;
        }

        public async Task<LiveTimetable> GetTimetable(int sipStopId)
        {
            var client = new HttpClient
            {
                BaseAddress = new Uri(_sipTimetableUrl)
            };
            var requestArgs = $"?stop={sipStopId}";
            var response = await client.GetAsync(requestArgs);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            var parsedResponse = _parser.Parse(responseBody);
            return parsedResponse;
        }
    }
}
