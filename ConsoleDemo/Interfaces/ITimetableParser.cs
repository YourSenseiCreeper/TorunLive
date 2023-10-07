using TorunLive.Domain.Entities;

namespace ConsoleDemo.Interfaces
{
    public interface ITimetableParser
    {
        public Timetable Parse(string data);
    }
}
