namespace TorunLive.Application.Extensions
{
    public static class DayMinuteExtension
    {
        public static int ToDayMinute(this DateTime date)
        {
            return date.Hour * 60 + date.Minute;
        }

        public static DateTime GetDateTimeFromDayMinute(this int dayMinute)
        {
            var now = DateTime.Now;
            var hour = Math.Floor(dayMinute / (double)60);
            var minute = dayMinute % 60;
            var nowDate = new DateTime(now.Year, now.Month, now.Day, (int)hour, minute, 0);
            return nowDate;
        }
    }
}
