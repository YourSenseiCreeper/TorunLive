namespace TorunLive.Domain.Entities
{
    public class CompareArrival
    {
        public int Order { get; set; }
        public string StopName { get; set; } = string.Empty;
        public string StopId { get; set; } = string.Empty;
        public int BaseDayMinute { get; set; }
        public int? LiveDayMinute { get; set; }
        public int? Delay { get; set; }
    }
}
