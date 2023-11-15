using Microsoft.Extensions.Configuration;
using TorunLive.SIPTimetableScanner.Interfaces.Services;

namespace TorunLive.SIPTimetableScanner.Services
{
    public class DelayService : IDelayService
    {
        private readonly int _requestDelayBaseInMiliseconds;
        public DelayService(IConfiguration configuration)
        {
            var delayString = configuration[ConfigurationKeys.RequestDelayBaseInMiliseconds];
            _requestDelayBaseInMiliseconds = string.IsNullOrEmpty(delayString) ? 0 : int.Parse(delayString);
        }

        public async Task Delay()
        {
            var innerDelay = (int)(_requestDelayBaseInMiliseconds * new Random().NextDouble());
            await Task.Delay(innerDelay);
        }
    }
}
