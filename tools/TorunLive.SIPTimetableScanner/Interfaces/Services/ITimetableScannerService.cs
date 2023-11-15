namespace TorunLive.SIPTimetableScanner.Interfaces.Services
{
    public interface ITimetableScannerService
    {
        Task ScanTimetablesAndLines(string[] lines);
    }
}