using ConsoleDemo.Interfaces;
using TorunLive.Domain.Enums;

namespace ConsoleDemo.Services
{
    public class FullTimetableService : IFullTimetableService
    {
        public async Task GetFullTimetable()
        {
            var startStopId = 679;
            var now = DateTime.Now;
            var dayOfWeek = now.DayOfWeek.ToString();
            var polishDayOfWeek = (PolishDayOfWeek)Enum.Parse(typeof(PolishDayOfWeek), dayOfWeek);
            var dayMinute = now.Hour * 60 + now.Minute;

            var service = new TimetableService();
            var baseTimetable = await service.GetTimetable(startStopId, polishDayOfWeek, dayMinute);

            var liveTimetableService = new LiveTimetableService();
            var sipStopId = StopIdsMap.RozkladzikToSIP[startStopId];
            var liveTimetable = await liveTimetableService.GetTimetable(sipStopId);
            var comparator = new TimetableComparator();
            var result = comparator.Compare(baseTimetable, liveTimetable);
            foreach(var comparedLine in result)
            {
                Console.WriteLine($"Linia: {comparedLine.Number} - {comparedLine.Name}");
                foreach(var comparedArrival in comparedLine.Arrivals)
                {
                    var basic = DayMinuteToHourAndMinute(comparedArrival.BaseDayMinute);
                    var actual = DayMinuteToHourAndMinute(comparedArrival.ActualBaseMinute);
                    Console.WriteLine($"Planowy: {basic:hh:mm}, Aktualny: {actual:hh:mm}");
                }
                Console.WriteLine("-------------");
            }
        }

        private static DateTime DayMinuteToHourAndMinute(int dayMinute)
        {
            var now = DateTime.Now;
            var hour = Math.Floor((double)dayMinute / (double)60);
            var minute = dayMinute % 60;
            var nowDate = new DateTime(now.Year, now.Month, now.Day, (int) hour, minute, 0);
            return nowDate;
        }
    }
}
