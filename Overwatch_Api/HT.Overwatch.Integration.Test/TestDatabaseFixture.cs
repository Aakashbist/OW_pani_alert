using Microsoft.Extensions.Configuration;

namespace HT.Overwatch.Integration.Test;

public class TestDatabaseFixture
{
    private static readonly object _lock = new();
    private static bool _databaseInitialized;

    public TestDatabaseFixture()
    {
        lock (_lock)
        {
            if (!_databaseInitialized)
            {

                using (var context = CreateContext())
                {
                    context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();

                    context.AddRange(TestData.GetRegions());
                    context.AddRange(TestData.GetSites());
                    context.AddRange(TestData.GetQuickLinks());
                    context.AddRange(TestData.GetParameters());
                    context.AddRange(TestData.GetLocations());
                    context.AddRange(TestData.GetTimeSeries());
                    context.AddRange(TestData.GetTimeSeriesValues());
                    context.SaveChanges();
                }

                _databaseInitialized = true;
            }
        }
    }

    public OverwatchDbContext CreateContext()
    {
        var configuration = new ConfigurationBuilder()
                   .AddJsonFile("appsettings.json")
                   .Build();

        var connectionString = configuration.GetConnectionString("TestConnectionString");

        return new OverwatchDbContext(
             new DbContextOptionsBuilder<OverwatchDbContext>()
                 .UseNpgsql(connectionString)
                 .Options);
    }
}

