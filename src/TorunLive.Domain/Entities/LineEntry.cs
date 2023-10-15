namespace TorunLive.Domain.Entities
{
    public class LineEntry
    {
        public string Name { get; set; }
        public string DirectionName { get; set; }
        public string TimetableUrl { get; set; }
        public List<TimetableEntry> Timetable { get; set; }
    }
}
