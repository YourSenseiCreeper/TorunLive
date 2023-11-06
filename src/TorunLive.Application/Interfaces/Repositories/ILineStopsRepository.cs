using TorunLive.Domain.Entities;

namespace TorunLive.Application.Interfaces.Repositories
{
    public interface ILineStopsRepository
    {
        public LineEntry? GetForLineAndDirection(string lineName, string lineDirection);
    }
}