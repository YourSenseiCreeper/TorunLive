namespace TorunLive.Domain.Entities
{
    public class LineDirection
    {
        public string LineName { get; set; }
        public IEnumerable<string> Directions { get; set; }
    }
}
