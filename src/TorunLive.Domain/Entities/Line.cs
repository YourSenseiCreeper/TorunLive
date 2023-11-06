namespace TorunLive.Domain.Entities
{
    public class Line
    {
        public string Number { get; set; }
        public string Name { get; set; }
        public int UnknownOne { get; set; }
        public int UnknownTwo { get; set; }
        public List<Arrival> Arrivals { get; set; }
        public string? Message { get; set; }
    }
}
