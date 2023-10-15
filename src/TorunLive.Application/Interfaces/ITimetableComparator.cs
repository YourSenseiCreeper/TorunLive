using TorunLive.Domain.Entities;

namespace TorunLive.Application.Interfaces
{
    public interface ITimetableComparator
    {
        public List<CompareLine> Compare(Timetable baseTimetable, LiveTimetable liveTimetable);
    }
}
