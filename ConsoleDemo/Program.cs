using ConsoleDemo;
using ConsoleDemo.Parsers;
using ConsoleDemo.Services;
using TorunLive.Domain.Enums;

Console.WriteLine("Hello, World!");
var service = new TimetableService();
//var startStopId = 679;
//var dayOfWeek = PolishDayOfWeek.Friday;
//var dayMinute = 1352;
//service.GetTimetable(startStopId, dayOfWeek, dayMinute).GetAwaiter().GetResult();
//var timetableParser = new TimetableParser();
//timetableParser.Parse("17;1;16#$##$#|18;1;27#$##$#|25;1;18#$##$#|26;1;17;1360;4;213;;283;4;215;;325;4;216;;370;4;217;;415;4;218;#$##$#|34;1;21;1372;4;208;;296;4;210;;337;4;211;;382;4;212;;427;4;213;#$##$#|40;1;19;1386;4;218;0;313;4;220;0;334;4;221;;352;4;222;;397;4;223;#$#S#$#kurs skrócony do Placu Skalskiego|131;0;23;1369;4;168;0;332;4;171;;372;4;172;;412;4;173;0;452;4;174;#$#R#$#kurs wykonywany do Przysieka i Rozgart|N90;1;18;1424;4;34;;34;4;35;;84;4;36;;119;4;37;;179;4;38;#$##$#|N95;1;28;1436;4;15;;96;4;16;0;226;4;17;#$#S#$#kurs skrócony do pętli Plac Skalskiego");
//var liveTimetableParser = new LiveTimetableParser();
//liveTimetableParser.Parse("");
//var liveTimetableService =  new LiveTimetableService();
//liveTimetableService.GetTimetable(64301).GetAwaiter().GetResult();

var fullService = new FullTimetableService();
fullService.GetFullTimetable().GetAwaiter().GetResult();