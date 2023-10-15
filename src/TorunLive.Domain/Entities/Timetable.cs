namespace TorunLive.Domain.Entities
{
    public class Timetable
    {
        public int StartId { get; set; }
        public List<Line> Lines { get; set; }
    }
}
