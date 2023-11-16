namespace TorunLive.Domain.Entities
{
    public class LiveLine
    {
        public string Number { get; set; }
        public string Name { get; set; }
        public List<int> ArrivalsInDayMinutes { get; set; }
        public string? Message { get; set; }
    }
}
