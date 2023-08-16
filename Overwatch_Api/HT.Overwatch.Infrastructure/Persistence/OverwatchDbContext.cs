using HT.Overwatch.Domain.Model;
using HT.Overwatch.Domain.Model.Metadata;
using HT.Overwatch.Domain.Model.TimeSeriesStorage;
using Microsoft.EntityFrameworkCore;

namespace HT.Overwatch.Infrastructure.Persistence
{
    public class OverwatchDbContext : DbContext
    {
        public OverwatchDbContext(DbContextOptions<OverwatchDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Site>(entity =>
            {
                entity.HasOne(x => x.Region)
                        .WithMany(x => x.Sites)
                        .HasForeignKey(x => x.RegionId);
            });

            modelBuilder.Entity<Location>(entity =>
            {
                entity.HasOne(x => x.Site)
                        .WithMany(x => x.Locations)
                        .HasForeignKey(x => x.SiteId);
            });

            modelBuilder.Entity<TimeSeriesComment>(entity =>
            {
                entity.HasKey(tv => new { tv.TimeStart, tv.TimeSeriesId });
                entity.HasOne(x => x.TimeSeries)
                        .WithMany(x => x.TimeSeriesComments)
                        .HasForeignKey(x => x.TimeSeriesId);
            });

            modelBuilder.Entity<TimeSeriesValue>(entity =>
            {
                entity.HasKey(tv => new { tv.Time, tv.TimeSeriesId });

                entity.HasOne(x => x.TimeSeries)
                        .WithMany(x => x.TimeSeriesValues)
                        .HasForeignKey(x => x.TimeSeriesId);
            });

            modelBuilder.Entity<TimeseriesDataRetrival>(entity =>
            {
                entity.HasNoKey();
            });

            modelBuilder.Entity<TimeSeries>(entity =>
            {
                entity.HasOne(q => q.Location).WithMany().HasForeignKey(b => b.LocationId);
                entity.HasOne(q => q.Parameter).WithMany().HasForeignKey(b => b.ParameterId);
            });

            modelBuilder.Entity<QuickLink>(entity =>
            {
                entity.HasOne(x => x.Site)
                        .WithMany(x => x.QuickLinks)
                        .HasForeignKey(x => x.SiteId);
            });

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSnakeCaseNamingConvention();
        }
        public DbSet<Region> Regions { get; set; }
        public DbSet<TimeseriesDataRetrival> TimeseriesDataRetrivals { get; set; }
        public DbSet<Site> Sites { get; set; }
        public DbSet<TimeSeries> TimeSeries { get; set; }
        public DbSet<Location> Locations { get; set; }
        public DbSet<Parameter> Parameters { get; set; }
        public DbSet<QuickLink> QuickLinks { get; set; }
        public DbSet<TimeSeriesValue> TimeSeriesValues { get; set; }
        public DbSet<TimeSeriesComment> TimeSeriesComments { get; set; }
    }
}
