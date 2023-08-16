using HT.Overwatch.Contract.DTO;
using static HT.Overwatch.Application.Features.SiteTimeSeries.GetFilterTimeseries;
using Query = HT.Overwatch.Application.Features.SiteTimeSeries.GetFilterTimeseries.Query;

namespace HT.Overwatch.Integration.Test.Features.SiteTimeSeries;

public class GetTimeSeriesTest : IClassFixture<TestDatabaseFixture>
{
    public GetTimeSeriesTest(TestDatabaseFixture fixture)
    {
        Fixture = fixture;
    }
    private TestDatabaseFixture Fixture { get; }

    [Fact]
    public async Task Handler_Should_ThrowNpgsqlPostgresException_GivenResolution_LessThanTwo()
    {
        //Arrange
        var ctx = Fixture.CreateContext();
        var handler = new Handler(new TimeSeriesValuesRepository(ctx), new UnitOfWork(ctx));
        FilterTimeseriesOptions filterOptions = new()
        {
            FromDate = DateTime.Now.ToUniversalTime(),
            ToDate = DateTime.Now.AddHours(4).ToUniversalTime(),
            Point = 1,
            RowFilterOptions = TestData.CreateRowFilterOptions()
        };

        var query = new Query
        {
            FilterTimeseriesOptions = filterOptions
        };


        // Act 
        var exception = await Assert.ThrowsAsync<Npgsql.PostgresException>(() => handler.Handle(query, CancellationToken.None));

        //Assert
        Assert.Equal("XX000: resolution must be greater than 2", exception.Message);
    }


    [Theory]
    [InlineData(3)]
    [InlineData(6)]
    [InlineData(10)]
    public async Task Handler_Should_Return_SameCountOfTimeseriesData_GivenSpecificResolution(int value)
    {
        // Arrange
        var ctx = Fixture.CreateContext();
        var handler = new Handler(new TimeSeriesValuesRepository(ctx), new UnitOfWork(ctx));
        string expectedKey = "Bradys Dam/At Dam/Water Level (meter)";
        FilterTimeseriesOptions filterOptions = new()
        {
            FromDate = DateTime.Now.ToUniversalTime(),
            ToDate = DateTime.Now.AddHours(4).ToUniversalTime(),
            Point = value,
            RowFilterOptions = new List<RowFilterOptions> { TestData.CreateRowFilterOptions()[0] }
        };

        var query = new Query
        {
            FilterTimeseriesOptions = filterOptions
        };

        // Act
        var result = await handler.Handle(query, CancellationToken.None);

        //Assert
        Assert.NotNull(result);
        Assert.Contains(expectedKey, result.First().key);
        Assert.Equal(value, result.First().SiteTimeseriesItems.Count);
    }

    [Fact]
    public async Task Handler_Should_Return_True_TimeseriesValuesBetweenGivenTime()
    {
        //Arrange
        var ctx = Fixture.CreateContext();
        var handler = new Handler(new TimeSeriesValuesRepository(ctx), new UnitOfWork(ctx));
        var fromDate = DateTime.Now.ToUniversalTime();
        var toDate = DateTime.Now.AddHours(4).ToUniversalTime();

        FilterTimeseriesOptions filterOptions = new()
        {
            FromDate = fromDate,
            ToDate = toDate,
            Point = 3,
            RowFilterOptions = new List<RowFilterOptions> { TestData.CreateRowFilterOptions()[0] }
        };

        var query = new Query
        {
            FilterTimeseriesOptions = filterOptions
        };

        // Act 
        var result = await handler.Handle(query, CancellationToken.None);

        //Assert
        Assert.NotNull(result);
        Assert.True(result.First().SiteTimeseriesItems.All(x => x.Time >= fromDate && x.Time <= toDate));
    }

    [Fact]
    public async Task Handler_Should_Return_False_TimeseriesValuesBetweenGivenTime()
    {
        //Arrange
        var ctx = Fixture.CreateContext();
        var handler = new Handler(new TimeSeriesValuesRepository(ctx), new UnitOfWork(ctx));
        var fromDate = DateTime.Now.ToUniversalTime();
        var toDate = DateTime.Now.AddHours(4).ToUniversalTime();
        FilterTimeseriesOptions filterOptions = new()
        {
            FromDate = fromDate,
            ToDate = toDate,
            Point = 3,
            RowFilterOptions = new List<RowFilterOptions> { TestData.CreateRowFilterOptions()[0] }
        };

        var query = new Query
        {
            FilterTimeseriesOptions = filterOptions
        };


        // Act 
        var result = await handler.Handle(query, CancellationToken.None);

        //Assert
        Assert.NotNull(result);
        Assert.False(result.First().SiteTimeseriesItems.All(x => x.Time <= fromDate && x.Time >= toDate));
    }

    [Fact]
    public async Task Handler_Should_Return_TwoGroupOfTimeSeries_GivenOneSiteAndDifferentParameter()
    {
        //Arrange
        var ctx = Fixture.CreateContext();
        var handler = new Handler(new TimeSeriesValuesRepository(ctx), new UnitOfWork(ctx));
        string[] expectedKey = new[] { "Bradys Dam/At Dam/Water Level (meter)", "Bradys Dam/At Dam Leakage Weir/Channel Level (meter)" };

        FilterTimeseriesOptions filterOptions = new()
        {
            FromDate = DateTime.Now.ToUniversalTime(),
            ToDate = DateTime.Now.AddHours(4).ToUniversalTime(),
            Point = 3,
            RowFilterOptions = TestData.CreateRowFilterOptions()
        };

        var query = new Query
        {
            FilterTimeseriesOptions = filterOptions
        };

        // Act 
        var result = await handler.Handle(query, CancellationToken.None);

        //Assert
        Assert.NotNull(result);
        foreach (var item in result)
        {
            Assert.Contains(item.key, expectedKey);
        }
    }

}
