using System.Text;
using TorunLive.SIPStopScanner;

Console.OutputEncoding = Encoding.UTF8;
Console.WriteLine("Hello, World!");
var service = new StopsScannerService();
service.ScanStops().GetAwaiter().GetResult();
