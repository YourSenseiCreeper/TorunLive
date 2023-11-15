using Microsoft.Extensions.Configuration;
using TorunLive.SIPTimetableScanner.Interfaces.Services;

namespace TorunLive.SIPTimetableScanner.Services
{
    public class RequestService : IRequestService
    {
        private readonly HttpClient _httpClient;
        public RequestService(IConfiguration configuration)
        {
            var url = configuration[Configuration.ServiceUrl] ?? throw new ArgumentException("Missing service url in configuration");
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(url)
            };
        }

        public Task<string> GetTimetable(string lineId, int directionId, string stopId)
        {
            return GetTimetable($"{lineId}/{directionId}/{stopId}.html");
        }

        public async Task<string> GetTimetable(string timetableUrl)
        {
            var response = await _httpClient.GetAsync(timetableUrl);
            response.EnsureSuccessStatusCode();
            var htmlString = await response.Content.ReadAsStringAsync();
            return htmlString;
        }

        public async Task<string> GetLineDirections(string lineName)
        {
            var response = await _httpClient.GetAsync($"Linia{lineName}.html");
            response.EnsureSuccessStatusCode();

            var htmlString = await response.Content.ReadAsStringAsync();
            return htmlString;
        }
    }
}
