using TorunLive.Domain.Entities;

namespace TorunLive.Application.Interfaces
{
    public interface ILineStopsService
    {
        List<TimetableEntry> GetEntriesBeforeStop(string lineName, string direction, string stopId, int amountStopsBefore);
        void LoadFromFile(string filename);
    }
}