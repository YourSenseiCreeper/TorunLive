using TorunLive.SIPTimetableScanner.Entities;

namespace TorunLive.SIPTimetableScanner.Interfaces
{
    public interface ITimetableService
    {
        Task<List<(string, string)>> GetLineStopsUrls(string targetLineUrl);
        Task ScanLineDirectionStopAndStopTimes(LineDirection direction, List<string> lineUrls);
    }
}