namespace TorunLive.Domain.EntitiesV2
{
    public class LineStopTime
    {
        public int Id { get; set; }
        public int LineStopId { get; set; }
        public int DayMinute { get; set; }
        public bool IsWeekday { get; set; }
        public bool IsWinterHoliday { get; set; }
        public bool IsSaturdaySundayHoliday { get; set; }

        public LineStop LineStop { get; set; }
    }
}
