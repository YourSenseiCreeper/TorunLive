using Newtonsoft.Json;
using System;
using System.Xml.Linq;
using System.Xml.XPath;
using TorunLive.Domain.Entities;
using TorunLive.Domain.EntitiesV2;
using TorunLive.Persistance;

namespace TorunLive.SIPTimetableScanner.Services
{
    public class TimetableScannerService
    {
        private readonly TorunLiveContext _dbContext;
        private readonly HttpClient _httpClient;
        private readonly LineDirectionService _lineDirectionService;

        private static readonly string[] _lines = new[]
        {
            "1", "2", "3", "4", "5", "6", "7",
            "10", "11", "12", "13", "14", "15", "16", "17",
            "18", "19", "20", "24", "25", "26", "27", "28",
            "29", "30", "31", "32", "33", "34", "38", "39",
            "40", "41", "42", "44", "111", "112", "113", "115",
            "121", "122", "131", "N90", "N91", "N93", "N94", "N95"
        };
        private static readonly Dictionary<string, string> _xmlEscapeReplacements = new()
        {
            { "<br>", "<br></br>" },
            { "&", "&amp;" }
        };

        public TimetableScannerService(TorunLiveContext dbContext)
        {
            _dbContext = dbContext;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://rozklad.mzk-torun.pl/")
            };
            _lineDirectionService = new LineDirectionService(_httpClient, _dbContext);
        }

        public async Task ScanTimetablesAndLines()
        {
            var random = new Random();
            var allLinesAndDirections = new List<LineEntry>();
            try
            {
                foreach (var line in _lines)
                {
                    Console.WriteLine($"Scanning line {line}");
                    var directions = await GetLineDirections(line);
                    var lineUrls = directions.Select(d => d.Url).ToList();

                    foreach (var direction in directions)
                    {
                        await _lineDirectionService.ScanLineDirectionStopAndStopTimes(direction, lineUrls);
                    }

                    var delay = (int)(250 * random.NextDouble());
                    await Task.Delay(delay);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public async Task<IEnumerable<Entities.LineDirection>> GetLineDirections(string lineName)
        {
            var response = await _httpClient.GetAsync($"Linia{lineName}.html");
            response.EnsureSuccessStatusCode();

            var lineData = await response.Content.ReadAsStringAsync();
            var substring = Common.GetTextBetweenAndClean(lineData, "<center><p>", "</a></center>", _xmlEscapeReplacements);
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
