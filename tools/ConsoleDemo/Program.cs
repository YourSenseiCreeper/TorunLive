using TorunLive.Application.Services;

//var startStopId = 679;
//var dayOfWeek = PolishDayOfWeek.Friday;
//var dayMinute = 1352;
//service.GetTimetable(startStopId, dayOfWeek, dayMinute).GetAwaiter().GetResult();
//Console.WriteLine("Podaj id przystanku z systemu SIP: ");
//var sipStopId = int.Parse(Console.ReadLine());

// w jakiś sposób fajnie tu będzie wykorzystać DI z ASP.NETu, aktualnie nie wiem jak to zrobić
var fullService = new FullTimetableService(new TimetableService(), new TimetableComparator(), new LiveTimetableService(), new LineStopsService());
//fullService.GetFullTimetable(sipStopId).GetAwaiter().GetResult();
fullService.GetLiveForLine("N90", "64301", "UNIWERSYTET").GetAwaiter().GetResult();