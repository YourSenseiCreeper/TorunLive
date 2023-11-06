namespace TorunLive.Domain.Entities
{
    public class CompareLine
    {
        public string Number { get; set; }
        public string Name { get; set; }
        public List<CompareArrival> Arrivals { get; set; }
        public string? Error { get; set; }
    }
}
