﻿using Newtonsoft.Json;
using System.Xml.Linq;
using System.Xml.XPath;
using TorunLive.Domain.Entities;

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

            string fileName = "timetables.json";
            var random = new Random();
            var allLinesAndDirections = new List<LineEntry>();
            try
            {
                foreach (var line in lines)
                {
                    Console.WriteLine($"Scanning line {line}");
                    var directions = await GetLineEntries(line);
                    foreach (var direction in directions)
                    {
                        Console.WriteLine($"Found '{direction.DirectionName}', scanning stops...");
                        var timetableStops = await GetTimetableStops(direction.TimetableUrl);
                        direction.Timetable = timetableStops;
                        allLinesAndDirections.Add(direction);
                        var innerDelay = (int)(100 * random.NextDouble());
                        await Task.Delay(innerDelay);
                    }

                    SaveToFile(fileName, allLinesAndDirections);
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
                SaveToFile(fileName, allLinesAndDirections);
            }
        }

        public async Task<List<TimetableEntry>> GetTimetableStops(string lineUrl)
        {
            var response = await _httpClient.GetAsync(lineUrl);
            response.EnsureSuccessStatusCode();

            var rawData = await response.Content.ReadAsStringAsync();
            var parsed = ParseTimetableData(rawData);
            return parsed;
        }

        public static List<TimetableEntry> ParseTimetableData(string lineData)
        {
            var entries = new List<TimetableEntry>();
            var start = "<div class=\"timetable-stops\">";
            var startIndex = lineData.IndexOf(start);
            var endMarker = "<div class=\"timetable\">";
            var endIndex = lineData.IndexOf(endMarker);
            var substring = lineData.Substring(startIndex, endIndex - startIndex);
            substring = substring.Replace("<br>", "<br></br>").Replace("&", "&amp;");

            var document = XDocument.Parse(substring);
            var urls = document.XPathSelectElements("div/div/table/tr/td/a").ToList();
            var times = document.XPathSelectElements("div/div/table/tr/td")
                .Where(t => t.Attributes().Any(a => a.Value == "czas")).ToList();
            foreach (var (url, time) in urls.Zip(times))
            {
                var lineStopUrl = url.Attributes().FirstOrDefault(a => a.Name == "href")?.Value ?? string.Empty;
                lineStopUrl = lineStopUrl.Replace(".html", "");
                int stopId = string.IsNullOrEmpty(lineStopUrl) ? 0 : int.Parse(lineStopUrl);
                if (!int.TryParse(time.Value, out int timeElapsedFromFirstStop))
                {
                    Console.WriteLine($"Cannot parse timeElapsed value: '{time.Value}'");
                    timeElapsedFromFirstStop = 0;
                }
                entries.Add(new TimetableEntry
                {
                    Name = url.Value.Replace("&amp;", "&"),
                    StopId = stopId,
                    TimeElapsedFromFirstStop = timeElapsedFromFirstStop
                });
            }

            // ostatnią trzeba dodać manualnie, bo nie ma linku
            var cells = document.XPathSelectElements("div/div/table/tr/td").ToList();
            var lastCell = cells.LastOrDefault();
            if (lastCell != null)
            {
                entries.Add(new TimetableEntry
                {
                    Name = lastCell.Value.Replace("&amp;", "&"),
                });
            }
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

        public static void SaveToFile(string filename, List<LineEntry> lineEntries)
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            var path = Path.Combine(currentDirectory, filename);
            var serialized = JsonConvert.SerializeObject(lineEntries);
            File.WriteAllText(path, serialized);
        }
    }
}
