namespace TorunLive.Domain.Entities
{
    public class CompareArrival
    {
        public int StopId { get; set; }
        public int BaseDayMinute { get; set; }
        public int ActualBaseMinute { get; set; }
        public int? Delay { get; set; }
    }
}
