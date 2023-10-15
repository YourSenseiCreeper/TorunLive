using System.Collections.Specialized;
using System.Runtime.Caching;
using TorunLive.Domain.Entities;

namespace TorunLive.Application.Cache
{
    public class TimetableCache : MemoryCache
    {
        public TimetableCache(string name, NameValueCollection config = null) : base(name, config)
        {
        }

        // z cache wyciągam obiekty typu object - rzutowanie ich na Timetable może trochę zająć i nie być tak optymalne
        // TODO: trzeba zastanowić się jak zaprojektować ten cache i jak mają wyglądać obiekty
        public Timetable GetTimetable(string line, string direction)
        {
            return null;
        }
    }
}
