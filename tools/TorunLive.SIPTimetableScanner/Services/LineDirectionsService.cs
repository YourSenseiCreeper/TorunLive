using TorunLive.SIPTimetableScanner.Interfaces;

namespace TorunLive.SIPTimetableScanner.Services
{
    public class LineDirectionsService : ILineDirectionsService
    {
        private readonly IRequestService _requestService;
        private readonly ILineDirectionsParserService _parserService;

        public LineDirectionsService(
            IRequestService requestService,
            ILineDirectionsParserService parserService
            )
        {
            _requestService = requestService;
            _parserService = parserService;
        }

        public async Task<IEnumerable<Entities.LineDirection>> GetLineDirections(string lineName)
        {
            var htmlString = await _requestService.GetLineDirections(lineName);
            var parsed = _parserService.ParseLineDirections(lineName, htmlString);

            return parsed;
        }
    }
}
