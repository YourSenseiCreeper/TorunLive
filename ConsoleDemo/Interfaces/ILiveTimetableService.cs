using TorunLive.Domain.Entities;

namespace ConsoleDemo.Interfaces
{
    public interface ILiveTimetableService
    {
        public Task<LiveTimetable> GetTimetable(int sipStopId);
    }
}
