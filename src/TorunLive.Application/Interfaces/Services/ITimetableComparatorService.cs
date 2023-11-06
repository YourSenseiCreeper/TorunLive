using TorunLive.Domain.Entities;

namespace TorunLive.Application.Interfaces.Services
{
    public interface ITimetableComparatorService
    {
        public List<CompareLine> Compare(Timetable baseTimetable, LiveTimetable liveTimetable);
    }
}
