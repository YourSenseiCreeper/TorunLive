﻿using TorunLive.Domain.Entities;

namespace TorunLive.Application.Interfaces.Parsers
{
    public interface ITimetableParser
    {
        public Timetable Parse(string data);
    }
}