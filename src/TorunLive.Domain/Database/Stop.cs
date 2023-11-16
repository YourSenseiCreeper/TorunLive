namespace TorunLive.Domain.Database
{
    public class Stop
    {
        public string Id { get; set; }
        public string Name { get; set; }

        public ICollection<LineStop> LineStops { get; set; }
    }
}
