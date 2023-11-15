﻿using Microsoft.Extensions.Configuration;
using TorunLive.Application.Interfaces.Services;

namespace TorunLive.Application.Services
{
    public class LiveRequestService : ILiveRequestService
    {
        private readonly HttpClient _httpClient;
        public LiveRequestService(
            IConfiguration configuration
            )
        {
            var url = configuration[ConfigurationKeys.SipTimetableUrl] ?? throw new ArgumentException("Missing service url in configuration");
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(url)
            };
        }

        public async Task<string> GetTimetable(string stopId)
        {
            var response = await _httpClient.GetAsync($"?stopId={stopId}");
            response.EnsureSuccessStatusCode();
            var htmlString = await response.Content.ReadAsStringAsync();
            return htmlString;
        }

    }
}
