using FluentValidation.TestHelper;
using HT.Overwatch.Application.Features.QuickLinks;
using HT.Overwatch.Contract.Requests;

namespace HT.Overwatch.Integration.Test.Features.QuickLinks
{
    public class AddQuickLinkTest : IClassFixture<TestDatabaseFixture>
    {
        private TestDatabaseFixture Fixture { get; }

        public AddQuickLinkTest(TestDatabaseFixture fixture) =>
            Fixture = fixture;

        [Theory]
        [InlineData(7000, "Quick Link Test 1", "https://www.someurl.com", 4)]
        [InlineData(8000, "Quick Link Test 2", "https://www.someurl.com", 4)]
        [InlineData(9000, "Quick Link Test 3", "https://www.someurl.com", 4)]
        public async Task Handler_Should_AddNewQuickLink_GivenValidData(int id, string name, string url, int siteId)
        {
            //Arrange
            var ctx = Fixture.CreateContext();
            var handler = new AddQuickLink.Handler(new UnitOfWork(ctx));
            var request = new AddQuickLinkRequest() { Id = id, Name = name, Url = url, SiteId = siteId };

            //Act
            var result = await handler.Handle(new AddQuickLink.Command() { Request = request }, CancellationToken.None);

            // Asssert
            Assert.NotNull(result);
            Assert.True(result.Success);
        }

        [Theory]
        [InlineData(1000, "Quick Link Test 1", "", 4)]
        [InlineData(2000, null, "https://www.someurl.com", 4)]
        [InlineData(3000, "", null, 4)]
        [InlineData(4000, null, "", 4)]
        public async Task CommandValidator_Should_Fail_GivenInvalidData(int id, string name, string url, int siteId)
        {
            //Arrange
            var ctx = Fixture.CreateContext();
            var request = new AddQuickLinkRequest() { Id = id, Name = name, Url = url, SiteId = siteId };
            var command = new AddQuickLink.Command() { Request = request };
            var validator = new AddQuickLink.CommandValidator();

            //Act
            var result = await validator.TestValidateAsync(command);

            // Asssert
            Assert.NotEmpty(result.Errors);
        }

        [Fact]
        public async Task Handler_Should_ThrowException_GivenExistingId()
        {
            //Arrange
            var ctx = Fixture.CreateContext();
            var handler = new AddQuickLink.Handler(new UnitOfWork(ctx));
            var request = new AddQuickLinkRequest() { Id = 1000, Name = "Quick Link Test 1", Url = "https://www.someurl.com", SiteId = 0 };

            // Act & Assert
            await Assert.ThrowsAsync<DbUpdateException>(() => handler.Handle(new AddQuickLink.Command() { Request = request }, CancellationToken.None));
        }
    }
}
