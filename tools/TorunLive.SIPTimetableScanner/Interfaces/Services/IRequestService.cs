namespace TorunLive.SIPTimetableScanner.Interfaces.Services
{
    public interface IRequestService
    {
        Task<string> GetTimetable(string timetableUrl);
        Task<string> GetTimetable(string lineId, int directionId, string stopId);
        Task<string> GetLineDirections(string lineName);
    }
}