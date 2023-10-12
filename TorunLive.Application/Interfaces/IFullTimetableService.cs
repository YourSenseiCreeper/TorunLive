namespace TorunLive.Application.Interfaces
{
    public interface IFullTimetableService
    {
        public Task GetFullTimetable(int sipStopId);
    }
}
