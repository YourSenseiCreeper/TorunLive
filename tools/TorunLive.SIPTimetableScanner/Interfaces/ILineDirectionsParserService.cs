using TorunLive.SIPTimetableScanner.Entities;

namespace TorunLive.SIPTimetableScanner.Interfaces
{
    public interface ILineDirectionsParserService
    {
        IEnumerable<LineDirection> ParseLineDirections(string lineName, string htmlString);
    }
}