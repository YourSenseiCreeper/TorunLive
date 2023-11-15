using TorunLive.SIPTimetableScanner.Entities;

namespace TorunLive.SIPTimetableScanner.Interfaces.Services
{
    public interface ILineDirectionsService
    {
        Task<IEnumerable<LineDirection>> GetLineDirections(string lineName);
    }
}