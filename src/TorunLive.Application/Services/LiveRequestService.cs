using Microsoft.Extensions.Configuration;
using TorunLive.Application.Interfaces.Services;

namespace TorunLive.Application.Services
{
    public class LiveRequestService : ILiveRequestService
    {
        private readonly HttpClient _httpClient;
        private readonly string TimetableArg;

        public LiveRequestService(
            IConfiguration configuration
            )
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(configuration[ConfigurationKeys.SipTimetableUrl] ?? throw new ArgumentException("Missing service url in configuration")),
            };
            TimetableArg = configuration[ConfigurationKeys.SipTimetableArg] ?? throw new ArgumentException("Missing service url in configuration");
        }

        public async Task<string> GetTimetable(string stopId)
        {
            var response = await _httpClient.GetAsync(TimetableArg.Replace(ConfigurationKeys.SipTimetableArgReplacement, stopId));
            response.EnsureSuccessStatusCode();
            var htmlString = await response.Content.ReadAsStringAsync();
            return htmlString;
        }

    }
}
