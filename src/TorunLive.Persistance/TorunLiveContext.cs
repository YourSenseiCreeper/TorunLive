using Microsoft.EntityFrameworkCore;
using TorunLive.Domain.EntitiesV2;

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
                .HasKey(nameof(Direction.LineId), nameof(Direction.DirectionId));

            modelBuilder.Entity<LineStop>()
                .HasIndex(new string[] { nameof(LineStop.Id), }, "IX_LineStop_LineStop");

            modelBuilder.Entity<LineStop>()
                .HasIndex(new string[] { nameof(LineStop.LineId), nameof(LineStop.DirectionId), nameof(LineStop.StopId) });

            base.OnModelCreating(modelBuilder);
        }
    }
}