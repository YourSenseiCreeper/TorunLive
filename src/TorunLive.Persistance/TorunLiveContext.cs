using Microsoft.EntityFrameworkCore;
using TorunLive.Domain.Database;

namespace TorunLive.Persistance
{
    public class TorunLiveContext : DbContext
    {
        public TorunLiveContext(DbContextOptions<TorunLiveContext> options) : base(options)
        {
        }

        public DbSet<Line> Lines { get; set; }
        public DbSet<Stop> Stops { get; set; }
        public DbSet<Direction> Directions { get; set; }
        public DbSet<LineStop> LineStops { get; set; }
        public DbSet<LineStopTime> LineStopTimes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Direction>()
                .HasKey(t => new { t.LineId, t.DirectionId });

            modelBuilder.Entity<LineStop>()
                .HasIndex(t => new { t.Id }, "IX_LineStop_LineStop");

            modelBuilder.Entity<LineStop>()
                .HasIndex(t => new { t.DirectionLineId, t.DirectionId, t.StopId });

            base.OnModelCreating(modelBuilder);
        }
    }
}