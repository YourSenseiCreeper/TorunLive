using TorunLive.Domain.Entities;

namespace TorunLive.Application.Interfaces.Parsers
{
    public interface ILiveTimetableParser
    {
        public LiveTimetable Parse(string data);
    }
}
