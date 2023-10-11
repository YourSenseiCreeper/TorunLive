// See https://aka.ms/new-console-template for more information
using TorunLive.SIPTimetableScanner;

Console.WriteLine("Hello, World!");

var service = new TimetableScannerService();
service.ScanTimetablesAndLines().GetAwaiter().GetResult();