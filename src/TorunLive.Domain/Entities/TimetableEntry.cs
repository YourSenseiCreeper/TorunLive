namespace TorunLive.Domain.Entities
{
    public class TimetableEntry
    {
        public int Order { get; set; }
        public string Name { get; set; }
        public int StopId { get; set; }
        public int TimeElapsedFromFirstStop { get; set; }
    }
}
