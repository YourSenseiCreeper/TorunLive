using Microsoft.Extensions.Configuration;
using TorunLive.SIPTimetableScanner.Interfaces;

namespace TorunLive.SIPTimetableScanner.Services
{
    public class DelayService : IDelayService
    {
        private readonly int _requestDelayBaseInMiliseconds;
        public DelayService(IConfiguration configuration)
        {
            _requestDelayBaseInMiliseconds = configuration.GetValue<int>("RequestDelayBaseInMiliseconds");
        }

        public async Task Delay()
        {
            var innerDelay = (int)(_requestDelayBaseInMiliseconds * new Random().NextDouble());
            await Task.Delay(innerDelay);
        }
    }
}
