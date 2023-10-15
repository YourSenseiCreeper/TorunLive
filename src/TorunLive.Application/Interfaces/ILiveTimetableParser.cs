using TorunLive.Domain.Entities;

namespace TorunLive.Application.Interfaces
{
    public interface ILiveTimetableParser
    {
        public LiveTimetable Parse(string data);
    }
}
