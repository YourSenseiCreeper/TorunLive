using TorunLive.SIPTimetableScanner.Entities;

namespace TorunLive.SIPTimetableScanner.Interfaces.Services
{
    public interface ITimetableService
    {
        Task ScanLineDirectionStopAndStopTimes(LineDirection direction, List<string> lineUrls);
    }
}