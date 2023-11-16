using TorunLive.Application.Interfaces.Services;

namespace TorunLive.Application.Services
{
    public class DateTimeService : IDateTimeService
    {
        public DateTimeOffset Now
        {
            get => DateTimeOffset.Now;
        }
    }
}
