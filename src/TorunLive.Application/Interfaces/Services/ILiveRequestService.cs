namespace TorunLive.Application.Interfaces.Services
{
    public interface ILiveRequestService
    {
        Task<string> GetTimetable(string stopId);
    }
}