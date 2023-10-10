using System.Xml.Linq;
using System.Xml.XPath;

namespace TorunLive.SIPStopScanner
{
    public class LiveTimetableService
    {
        private readonly HttpClient _httpClient;
        public LiveTimetableService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://sip.um.torun.pl:8080/default.aspx")
            };
        }

        public async Task ScanStops()
        {
            string fileName = "stops.csv";
            var random = new Random();
            var foundStops = LoadFromFile(fileName);
            var retries = 0;
            var lastStopAmount = 0;
            var start = 800;
            var lastSaved = 0;
            try
            {
                for (int i = start; i < 1000; i++)
                {
                    string result = string.Empty;
                    for (int j = 1; j < 8; j++)
                    {
                        var stopNumber = $"{i}0{j}";
                        result = await GetStopName(stopNumber);
                        if (result != null)
                        {
                            foundStops.Add(stopNumber, result);
                            Console.WriteLine($"Found {stopNumber} - {result}");
                            break;
                        }
                        var innerDelay = (int) (100 * random.NextDouble());
                        await Task.Delay(innerDelay);
                    }

                    if (lastStopAmount == foundStops.Count)
                        retries++;
                    else
                    {
                        lastStopAmount = foundStops.Count;
                        retries = 0;
                    }

                    if (foundStops.Count - lastSaved > 100)
                    {
                        lastSaved = foundStops.Count;
                        SaveToFile("stops.csv", foundStops);
                    }

                    if (retries > 40)
                        break;

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
                SaveToFile("stops.csv", foundStops);
            }
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

        public async Task<string> GetStopName(string sipStopId)
        {
            var requestArgs = $"?t1=&t2={sipStopId}&t3=&t4=";
            var response = await _httpClient.GetAsync(requestArgs);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();
            if (responseBody.Contains("Nie ma takiego przystanku"))
                return null;

            var parsed = Parse(sipStopId, responseBody);
            return parsed;
        }

        public static string Parse(string sipStopId, string data)
        {
            var panelName = "<section class=\"grid_9\">";
            var startIndex = data.IndexOf(panelName);
            var endMarker = "<h1 style=\"margin-left: 87%\">";
            var endIndex = data.IndexOf(endMarker);
            var substring = data.Substring(startIndex, endIndex + endMarker.Length - startIndex);
            substring += "</h1></div></div></section>";
            substring = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" + substring;
            substring = substring.Replace("&", "&amp;");

            var document = XDocument.Parse(substring);
            var headings = document.XPathSelectElements("section/div/div/h1").ToList();
            var lineName = headings[0].Value.Trim();
            lineName = lineName.Replace($"{sipStopId} - ", "");
            return lineName;
        }
    }
}
