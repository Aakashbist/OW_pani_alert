using HT.Overwatch.Application.Features.QuickLinks;

namespace HT.Overwatch.Integration.Test.Features.QuickLinks
{
    public class GetQuickLinksTest : IClassFixture<TestDatabaseFixture>
    {

        public GetQuickLinksTest(TestDatabaseFixture fixture) =>
            Fixture = fixture;
        private TestDatabaseFixture Fixture { get; }

        [Theory]
        [InlineData(1, 2)]
        [InlineData(2, 3)]
        public async Task Handler_SameCountOfQuickLinks_GivenSpecificExpectedResult(int siteId, int expectedResult)
        {
            //Arrange
            var ctx = Fixture.CreateContext();
            var handler = new GetQuickLinksBySite.Handler(new UnitOfWork(ctx));
            //Act
            var result = await handler.Handle(new GetQuickLinksBySite.Query() { SiteId = siteId }, CancellationToken.None);
            // Asssert
            Assert.NotNull(result);
            Assert.Equal(result.Count(), expectedResult);
        }

        //[Fact]
        //public async Task Handler_Should_ThrowNotFoundException_GivenSiteIdDidNotMatch()
        //{
        //    //Arrange
        //    var ctx = Fixture.CreateContext();
        //    var handler = new GetQuickLinksBySite.Handler(new UnitOfWork(ctx));
        //    int siteId = 10000000;
        //    // Act 
        //    var exception = await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(new GetQuickLinksBySite.Query() { SiteId = siteId }, CancellationToken.None));

        //    //Assert
        //    Assert.Equal($"Entity QuickLinks with field ({siteId}) was not found.", exception.Message);
        //}


    }
}
