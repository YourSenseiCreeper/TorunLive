namespace TorunLive.Domain.Entities
{
    public class CompareLine
    {
        public string Number { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Direction { get; set; } = string.Empty;
        public List<CompareArrival> Arrivals { get; set; }
        public string? Error { get; set; }
    }
}
