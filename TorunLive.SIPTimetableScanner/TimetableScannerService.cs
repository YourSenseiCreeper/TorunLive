using System.Xml.Linq;
using System.Xml.XPath;

namespace TorunLive.SIPTimetableScanner
{
    public class TimetableScannerService
    {
        private readonly HttpClient _httpClient;
        public TimetableScannerService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://rozklad.mzk-torun.pl/")
            };
        }

        // stąd można zescrapowć wszystkie trasy wraz z ID przystanków zgodnymi z SIP
        // przydatne przy śledzeniu na żywo autobusu na wybranej trasie
        // 

        public async Task ScanTimetablesAndLines()
        {
            var lines = new[]
            {
                "1", "2", "3", "4", "5", "6", "7",
                "10", "11", "12", "13", "14", "15", "16", "17",
                "18", "19", "20", "24", "25", "26", "27", "28",
                "29", "30", "31", "32", "33", "34", "38", "39",
                "40", "41", "42", "44", "111", "112", "113", "115",
                "121", "122", "131", "N90", "N91", "N93", "N94", "N95"
            };

            string fileName = "timetables.csv";
            var random = new Random();
            //var foundStops = LoadFromFile(fileName);
            try
            {
                foreach (var line in lines)
                {
                    var directions = await GetLineEntries(line);
                    foreach (var direction in directions)
                    {
                        var rawTimetable = await GetTimetableStops(direction.DirectionName, direction.TimetableUrl);
                        // parse
                        // add to dictionary
                        //result = await GetStopName(stopNumber);
                        //if (result != null)
                        //{
                        //    foundStops.Add(stopNumber, result);
                        //    Console.WriteLine($"Found {stopNumber} - {result}");
                        //    break;
                        //}
                        var innerDelay = (int)(100 * random.NextDouble());
                        await Task.Delay(innerDelay);
                    }

                    //SaveToFile("stops.csv", foundStops);
                    var delay = (int)(250 * random.NextDouble());
                    await Task.Delay(delay);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            finally
            {
                //SaveToFile(fileName, foundStops);
            }
        }

        public async Task<List<TimetableEntry>> GetTimetableStops(string lineDirection, string lineUrl)
        {
            var entries = new List<TimetableEntry>();
            var response = await _httpClient.GetAsync(lineUrl);
            response.EnsureSuccessStatusCode();

            var rawData = await response.Content.ReadAsStringAsync();
            var parsed = ParseTimetableData(lineDirection, rawData);
            return entries;
        }

        public List<TimetableEntry> ParseTimetableData(string lineDirection, string lineData)
        {
            var entries = new List<TimetableEntry>();
            var start = "<div class=\"timetable-stops\">";
            var startIndex = lineData.IndexOf(start);
            var endMarker = "<div class=\"timetable\">";
            var endIndex = lineData.IndexOf(endMarker);
            var substring = lineData.Substring(startIndex, endIndex - startIndex);
            substring = substring.Replace("<br>", "<br></br>").Replace("&", "&amp;");

            var document = XDocument.Parse(substring);
            //var tableRows = document.XPathSelectElements("div/div/table/tr").ToList();
            var urls = document.XPathSelectElements("div/div/table/tr/td/a").ToList();
            // trzeba manualnie dodać ostatni przystanek do listy
            return entries;
        }

        public async Task<List<LineEntry>> GetLineEntries(string lineName)
        {
            var response = await _httpClient.GetAsync($"Linia{lineName}.html");
            response.EnsureSuccessStatusCode();

            var rawData = await response.Content.ReadAsStringAsync();
            var parsed = GetLinesDirections(lineName, rawData);
            return parsed;
        }

        public static List<LineEntry> GetLinesDirections(string name, string lineData)
        {
            var entries = new List<LineEntry>();
            var start = "<center><p>";
            var startIndex = lineData.IndexOf(start);
            var endMarker = "</a></center>";
            var endIndex = lineData.IndexOf(endMarker);
            var substring = lineData.Substring(startIndex, endIndex + endMarker.Length - startIndex);
            substring = substring.Replace("<br>", "<br></br>").Replace("&", "&amp;");

            var ignoreUrls = new[]
            {
                "panel.html", "https://mzk-torun.pl"
            };

            var document = XDocument.Parse(substring);
            var urls = document.XPathSelectElements("center/a").ToList();
            foreach(var url in urls)
            {
                var attributes = url.Attributes();
                var hrefAttribute = attributes.FirstOrDefault(a => a.Name == "href")?.Value ?? null;
                if (ignoreUrls.Contains(hrefAttribute))
                    continue;

                var aValue = url.Value.Replace("<h3>", "").Replace("</h3>", "");
                entries.Add(new LineEntry
                {
                    Name = name,
                    DirectionName = aValue,
                    TimetableUrl = hrefAttribute
                });
            }
            return entries;
        }

        private static Dictionary<string, string> LoadFromFile(string filename)
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var path = Path.Combine(currentDirectory, filename);
            var lines = File.ReadAllLines(path);
            var result = lines.Select(l =>
            {
                var line = l.Split(',');
                return new { Key = line[0], Value = line[1] };
            }).ToDictionary(kv => kv.Key, kv => kv.Value);
            return result;
        }

        public static void SaveToFile(string filename, Dictionary<string, string> stops)
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var path = Path.Combine(currentDirectory, filename);
            var values = stops.Select(kv => $"{kv.Key},{kv.Value}");
            File.WriteAllLines(path, values);
        }
    }

    public class TimetableEntry
    {

    }

    public class LineEntry
    {
        public string Name { get; set; }
        public string DirectionName { get; set; }
        public string TimetableUrl { get; set; }
    }
}
