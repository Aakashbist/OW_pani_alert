using FluentValidation.TestHelper;
using HT.Overwatch.Application.Common.Exceptions;
using HT.Overwatch.Application.Features.QuickLinks;
using HT.Overwatch.Contract.Requests;
using HT.Overwatch.Domain.Model;

namespace HT.Overwatch.Integration.Test.Features.QuickLinks
{
    public class UpdateQuickLinkTest : IClassFixture<TestDatabaseFixture>
    {
        private TestDatabaseFixture Fixture { get; }

        public UpdateQuickLinkTest(TestDatabaseFixture fixture) =>
            Fixture = fixture;

        [Theory]
        [InlineData(5000, "Updated Name 1", "https://www.someurl.com", 4)]
        [InlineData(5000, "Updated Name 1", "https://www.updateurl.com", 4)]
        [InlineData(6000, "Updated Name 2", "https://www.updatedurl2.com", 4)]
        public async Task Handler_Should_UpdateQuickLink_GivenValidData(int id, string name, string url, int siteId)
        {
            //Arrange
            var ctx = Fixture.CreateContext();
            var handler = new UpdateQuickLink.Handler(new UnitOfWork(ctx));
            var request = new UpdateQuickLinkRequest() { Id = id, Name = name, Url = url, SiteId = siteId };

            //Act
            var result = await handler.Handle(new UpdateQuickLink.Command() { Request = request }, CancellationToken.None);

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
            var request = new UpdateQuickLinkRequest() { Id = id, Name = name, Url = url, SiteId = siteId };
            var command = new UpdateQuickLink.Command() { Request = request };
            var validator = new UpdateQuickLink.CommandValidator();

            //Act
            var result = await validator.TestValidateAsync(command);

            // Asssert
            Assert.NotEmpty(result.Errors);
        }

        [Fact]
        public async Task Handler_Should_ThrowException_GivenNonExistingId()
        {
            //Arrange
            var ctx = Fixture.CreateContext();
            var handler = new UpdateQuickLink.Handler(new UnitOfWork(ctx));
            var request = new UpdateQuickLink.Command() { Request = new UpdateQuickLinkRequest() { 
                Id = 12000, 
                SiteId = 4, 
                Url = "https://www.someupdatedurl.com", 
                Name = "Updated Name 5" } 
            };

            // Act & Assert
            await Assert.ThrowsAsync<DbUpdateConcurrencyException>(() => handler.Handle(request, CancellationToken.None));
        }
    }
}
