namespace TorunLive.Application.Extensions
{
    public static class DayMinuteExtension
    {
        public static int ToDayMinute(this DateTime date)
        {
            return date.Hour * 60 + date.Minute;
        }

        public static int ToDayMinute(this DateTimeOffset date)
        {
            return date.Hour * 60 + date.Minute;
        }

        public static DateTime GetDateTimeFromDayMinute(this int dayMinute, DateTime now)
        {
            var hour = Math.Floor(dayMinute / (double)60);
            var minute = dayMinute % 60;
            var nowDate = new DateTime(now.Year, now.Month, now.Day, (int)hour, minute, 0);
            return nowDate;
        }

        public static DateTimeOffset GetDateTimeOffsetFromDayMinute(this int dayMinute, DateTimeOffset now)
        {
            var hour = Math.Floor(dayMinute / (double)60);
            var minute = dayMinute % 60;
            var nowDate = new DateTimeOffset(now.Year, now.Month, now.Day, (int)hour, minute, 0, TimeSpan.Zero);
            return nowDate;
        }
    }
}
