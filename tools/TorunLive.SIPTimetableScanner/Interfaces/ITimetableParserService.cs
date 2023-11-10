using TorunLive.Domain.EntitiesV2;

namespace TorunLive.SIPTimetableScanner.Interfaces
{
    public interface ITimetableParserService
    {
        IEnumerable<LineStopTime> ParseArrivals(int lineStopId, string lineData);
        IEnumerable<(string, string)> ParseTimetablesUrls(string lineData);
    }
}