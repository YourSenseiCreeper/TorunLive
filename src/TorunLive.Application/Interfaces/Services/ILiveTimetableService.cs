using TorunLive.Domain.Entities;

namespace TorunLive.Application.Interfaces.Services
{
    public interface ILiveTimetableService
    {
        public Task<LiveTimetable> GetTimetable(int sipStopId);
    }
}
