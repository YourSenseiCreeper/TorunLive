namespace TorunLive.Application.Interfaces.Services
{
    public interface IFullTimetableService
    {
        public Task GetFullTimetable(int sipStopId);
        public Task GetLiveForLine(string lineNumber, string stopId, string direction);
    }
}
