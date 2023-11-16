using TorunLive.Domain.Database;
using TorunLive.SIPTimetableScanner.Entities;

namespace TorunLive.SIPTimetableScanner.Interfaces.Adapters
{
    public interface ITimetableAdapterService
    {
        IEnumerable<LineStopTime> ParseArrivals(int lineStopId, string lineData);
        IEnumerable<LineStopUrl> ParseTimetablesUrls(string lineData);
    }
}