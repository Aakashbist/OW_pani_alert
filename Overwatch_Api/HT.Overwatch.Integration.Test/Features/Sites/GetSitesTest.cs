using HT.Overwatch.Application.Features.Sites;

namespace HT.Overwatch.Integration.Test.Features.Sites
{
    public class GetSitesTest : IClassFixture<TestDatabaseFixture>
    {

        public GetSitesTest(TestDatabaseFixture fixture) =>
            Fixture = fixture;
        private TestDatabaseFixture Fixture { get; }

        [Fact]
        public async Task Handler_Should_ReturnAllSites()
        {
            //Arrange
            var ctx = Fixture.CreateContext();
            var handler = new GetSites.Handler(new UnitOfWork(ctx));
            //Act
            var result = await handler.Handle(new GetSites.Query(), CancellationToken.None);
            // Asssert
            Assert.NotNull(result);
            Assert.Equal(result.Count(), TestData.GetSites().Count);
        }

        [Fact]
        public async Task Handler_GivenSiteName_Should_ReturnAllSpecificSite()
        {
            //Arrange
            var ctx = Fixture.CreateContext();
            var handler = new GetSites.Handler(new UnitOfWork(ctx));
            //Act
            var result = await handler.Handle(new GetSites.Query() { Name = "Clark Dam" }, CancellationToken.None);
            //Asssert            
            Assert.NotNull(result);
            Assert.Equal("Clark Dam",result.First().Name);
            Assert.Single(result);
        }
    }
}
