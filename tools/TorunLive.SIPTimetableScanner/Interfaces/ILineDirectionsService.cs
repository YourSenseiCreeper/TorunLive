using TorunLive.SIPTimetableScanner.Entities;

namespace TorunLive.SIPTimetableScanner.Interfaces
{
    public interface ILineDirectionsService
    {
        Task<IEnumerable<LineDirection>> GetLineDirections(string lineName);
    }
}