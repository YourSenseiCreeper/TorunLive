using TorunLive.Domain.Entities;

namespace TorunLive.Application.Interfaces
{
    public interface ILiveTimetableService
    {
        public Task<LiveTimetable> GetTimetable(int sipStopId);
    }
}
