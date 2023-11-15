using System.Xml.Linq;
using System.Xml.XPath;
using TorunLive.SIPTimetableScanner.Entities;
using TorunLive.SIPTimetableScanner.Interfaces.Adapters;

namespace TorunLive.SIPTimetableScanner.Adapters
{
    public class LineDirectionsAdapterService : ILineDirectionsAdapterService
    {
        private static readonly string[] _ignoreUrls = new[]
        {
            "panel.html", "https://mzk-torun.pl"
        };

        public IEnumerable<LineDirection> ParseLineDirections(string lineName, string htmlString)
        {
            var substring = Common.GetTextBetweenAndClean(htmlString, "<center><p>", "</a></center>", Common.XmlEscapeReplacements);
            var document = XDocument.Parse(substring);
            var urls = document.XPathSelectElements("center/a").ToList();
            foreach (var url in urls)
            {
                var attributes = url.Attributes();
                var hrefAttribute = attributes.FirstOrDefault(a => a.Name == "href")?.Value ?? null;
                if (_ignoreUrls.Contains(hrefAttribute))
                    continue;

                var aValue = url.Value.Replace("<h3>", "").Replace("</h3>", "");
                yield return new LineDirection
                {
                    LineId = lineName,
                    DirectionName = aValue,
                    Url = hrefAttribute
                };
            }
        }
    }
}
