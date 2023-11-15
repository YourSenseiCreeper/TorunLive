using TorunLive.Domain.Entities;

namespace TorunLive.Application.Interfaces.Services
{
    public interface IFullTimetableService
    {
        public Task<IEnumerable<CompareLine>> GetFullTimetable(string sipStopId);
        public Task<CompareLine> GetLiveForLine(string lineNumber, string stopId, string direction);
    }
}
