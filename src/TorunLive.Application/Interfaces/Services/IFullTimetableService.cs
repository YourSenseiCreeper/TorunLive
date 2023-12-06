using TorunLive.Domain.Entities;

namespace TorunLive.Application.Interfaces.Services
{
    public interface IFullTimetableService
    {
        public Task<IEnumerable<CompareLine>> GetFullTimetable(string sipStopId);
        public Task<CompareLine> GetLiveForLine(string lineNumber, string stopId, int directionId);
        public Task<IEnumerable<DateTime>> GetNextArrivals(string lineNumber, int directionId, string stopId);
    }
}
