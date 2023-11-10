using TorunLive.SIPTimetableScanner.Interfaces;

namespace TorunLive.SIPTimetableScanner.Services
{
    public class DelayService : IDelayService
    {
        public async Task Delay()
        {
            var innerDelay = (int)(200 * new Random().NextDouble());
            await Task.Delay(innerDelay);
        }
    }
}
