using TorunLive.Domain.Entities;

namespace ConsoleDemo.Interfaces
{
    public interface ILiveTimetableParser
    {
        public LiveTimetable Parse(string data);
    }
}
