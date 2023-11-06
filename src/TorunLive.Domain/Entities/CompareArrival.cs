namespace TorunLive.Domain.Entities
{
    public class CompareArrival
    {
        public int Order { get; set; }
        public string StopName { get; set; } = string.Empty;
        public int StopId { get; set; }
        public int BaseDayMinute { get; set; }
        public int? ActualBaseMinute { get; set; }
        public int? Delay { get; set; }
    }
}
