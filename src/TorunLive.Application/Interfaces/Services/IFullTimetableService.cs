using TorunLive.Common;
using TorunLive.Domain.Entities;

namespace TorunLive.Application.Interfaces.Services
{
    public interface IFullTimetableService
    {
        public Task<IEnumerable<CompareLine>> GetFullTimetable(string sipStopId);
        public Task<Result<CompareLine>> GetLiveForLine(string lineNumber, string stopId, int directionId, int previousStopsNumber = 5);
        public Task<IEnumerable<DateTime>> GetNextArrivals(string lineNumber, int directionId, string stopId);
    }
}
