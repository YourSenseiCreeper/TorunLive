using TorunLive.SIPTimetableScanner.Interfaces.Adapters;
using TorunLive.SIPTimetableScanner.Interfaces.Services;

namespace TorunLive.SIPTimetableScanner.Services
{
    public class LineDirectionsService(
        IRequestService requestService,
        ILineDirectionsAdapterService parserService
    ) : ILineDirectionsService
    {
        private readonly IRequestService _requestService = requestService;
        private readonly ILineDirectionsAdapterService _parserService = parserService;

        public async Task<IEnumerable<Entities.LineDirection>> GetLineDirections(string lineName)
        {
            var htmlString = await _requestService.GetLineDirections(lineName);
            var parsed = _parserService.ParseLineDirections(lineName, htmlString);

            return parsed;
        }
    }
}
