using TorunLive.Domain.Entities;

namespace TorunLive.Application.Interfaces.Services
{
    public interface ILineStopsService
    {
        List<TimetableEntry> GetEntriesBeforeStop(string lineName, string direction, int stopId, int amountStopsBefore);
        List<LineDirection> GetLineDirectionsForStop(int stopId);
    }
}