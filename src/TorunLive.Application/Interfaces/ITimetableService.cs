using TorunLive.Domain.Entities;
using TorunLive.Domain.Enums;

namespace TorunLive.Application.Interfaces
{
    public interface ITimetableService
    {
        public Task<Timetable> GetTimetable(int startStopId, PolishDayOfWeek dayOfWeek, int dayMinute);
    }
}
