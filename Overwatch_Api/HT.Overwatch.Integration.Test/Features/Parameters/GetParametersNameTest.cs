using HT.Overwatch.Application.Common.Exceptions;
using static HT.Overwatch.Application.Features.Parameters.GetParameters;

namespace HT.Overwatch.Integration.Test.Features.Parameters;

public class GetParametersNameTest : IClassFixture<TestDatabaseFixture>
{
    public GetParametersNameTest(TestDatabaseFixture fixture)
    {
        Fixture = fixture;
    }
    private TestDatabaseFixture Fixture { get; }

    [Theory]
    [InlineData(4, 18, 1)]
    [InlineData(2, 11, 4)]
    public async Task Handler_Should_ReturnParameters_GivenSiteName(int siteId, int locationId, int count)
    {
        //Arrange
        var ctx = Fixture.CreateContext();
        var handler = new Handler(new UnitOfWork(ctx));
        //Act
        var result = await handler.Handle(new Query() { SiteId = siteId, LocationId = locationId }, CancellationToken.None);
        //Asssert
        Assert.NotNull(result);
        Assert.Equal(result.Count(), count);
    }

    [Theory]
    [InlineData(1234)]
    public async Task Handler_Should_ThrowNotFoundException_GivenSiteNameDidNotMatch(int siteId)
    {
        //Arrange
        var ctx = Fixture.CreateContext();
        var handler = new Handler(new UnitOfWork(ctx));
        // Act 
        var exception = await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(new Query() { SiteId = siteId }, CancellationToken.None));

        //Assert
        Assert.Equal($"Entity Parameters with field ({siteId}) was not found.", exception.Message);
    }

}