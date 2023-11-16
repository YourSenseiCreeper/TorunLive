namespace TorunLive.Domain.Database
{
    public class LineStopTime
    {
        public int Id { get; set; }
        public int LineStopId { get; set; }
        public int DayMinute { get; set; }
        public bool IsWeekday { get; set; }
        public bool IsWinterHoliday { get; set; }
        public bool IsSaturdaySundays { get; set; }
        public bool IsHolidays { get; set; }

        public LineStop LineStop { get; set; }
    }
}
