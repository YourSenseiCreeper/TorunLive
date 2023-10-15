namespace TorunLive.Domain.Entities
{
    public class TimetableEntry
    {
        public string Name { get; set; }
        public string StopId { get; set; }
        public int TimeElapsedFromFirstStop { get; set; }
    }
}
