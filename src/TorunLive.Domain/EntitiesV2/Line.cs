﻿namespace TorunLive.Domain.EntitiesV2
{
    public class Line
    {
        public string Id { get; set; }
        public string? Name { get; set; }

        public ICollection<LineStop> LineStops { get; set; }
        public ICollection<Direction> Directions { get; set; }
    }
}
