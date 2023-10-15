using TorunLive.Domain.Entities;

namespace TorunLive.Application.Interfaces
{
    public interface ITimetableParser
    {
        public Timetable Parse(string data);
    }
}
