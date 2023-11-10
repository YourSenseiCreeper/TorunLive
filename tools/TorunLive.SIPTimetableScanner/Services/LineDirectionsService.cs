using System.Xml.Linq;
using System.Xml.XPath;
using TorunLive.SIPTimetableScanner.Interfaces;

namespace TorunLive.SIPTimetableScanner.Services
{
    public class LineDirectionsService : ILineDirectionsService
    {
        private readonly IRequestService _requestService;
        public LineDirectionsService(
            IRequestService requestService
            )
        {
            _requestService = requestService;
        }

        public async Task<IEnumerable<Entities.LineDirection>> GetLineDirections(string lineName)
        {
            var htmlString = await _requestService.GetLineDirections(lineName);
            var substring = Common.GetTextBetweenAndClean(htmlString, "<center><p>", "</a></center>", Common.XmlEscapeReplacements);
            var ignoreUrls = new[]
            {
                "panel.html", "https://mzk-torun.pl"
            };

            var document = XDocument.Parse(substring);
            var urls = document.XPathSelectElements("center/a").ToList();
            var directions = new List<Entities.LineDirection>();
            foreach (var url in urls)
            {
                var attributes = url.Attributes();
                var hrefAttribute = attributes.FirstOrDefault(a => a.Name == "href")?.Value ?? null;
                if (ignoreUrls.Contains(hrefAttribute))
                    continue;

                var aValue = url.Value.Replace("<h3>", "").Replace("</h3>", "");
                directions.Add(new Entities.LineDirection
                {
                    LineId = lineName,
                    DirectionName = aValue,
                    Url = hrefAttribute
                });
            }

            return directions;
        }
    }
}
