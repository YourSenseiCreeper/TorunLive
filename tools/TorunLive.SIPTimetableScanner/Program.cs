using TorunLive.Persistance;
using TorunLive.SIPTimetableScanner.Services;

Console.WriteLine("Hello, World!");

var dbContext = new TorunLiveContext();
var service = new TimetableScannerService(dbContext);
service.ScanTimetablesAndLines().GetAwaiter().GetResult();