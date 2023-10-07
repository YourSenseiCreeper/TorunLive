using TorunLive.Domain.Entities;

namespace ConsoleDemo.Interfaces
{
    public interface ITimetableComparator
    {
        public List<CompareLine> Compare(Timetable baseTimetable, LiveTimetable liveTimetable);
    }
}
