using TorunLive.SIPTimetableScanner.Entities;

namespace TorunLive.SIPTimetableScanner.Interfaces.Adapters
{
    public interface ILineDirectionsAdapterService
    {
        IEnumerable<LineDirection> ParseLineDirections(string lineName, string htmlString);
    }
}