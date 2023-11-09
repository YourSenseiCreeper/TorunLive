// See https://aka.ms/new-console-template for more information
using TorunLive.Persistance;
using TorunLive.SIPTimetableScanner;

Console.WriteLine("Hello, World!");

var dbContext = new TorunLiveContext();
var service = new TimetableScannerService(dbContext);
service.ScanTimetablesAndLines().GetAwaiter().GetResult();