﻿using TorunLive.Domain.Entities;

namespace TorunLive.Application.Interfaces.Services
{
    public interface ITimetableComparatorService
    {
        public IEnumerable<CompareLine> Compare(LiveTimetable baseTimetable, LiveTimetable liveTimetable);
    }
}
