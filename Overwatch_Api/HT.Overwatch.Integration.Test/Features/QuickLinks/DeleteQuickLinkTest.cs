using FluentValidation.TestHelper;
using HT.Overwatch.Application.Common.Exceptions;
using HT.Overwatch.Application.Features.QuickLinks;

namespace HT.Overwatch.Integration.Test.Features.QuickLinks
{
    public class DeleteQuickLinkTest : IClassFixture<TestDatabaseFixture>
    {
        private TestDatabaseFixture Fixture { get; }

        public DeleteQuickLinkTest(TestDatabaseFixture fixture) =>
            Fixture = fixture;

        [Theory]
        [InlineData(1000)]
        [InlineData(2000)]
        [InlineData(3000)]
        public async Task Handler_Should_DeleteQuickLink_GivenValidId(int id)
        {
            //Arrange
            var ctx = Fixture.CreateContext();
            var handler = new DeleteQuickLink.Handler(new UnitOfWork(ctx));

            //Act
            var result = await handler.Handle(new DeleteQuickLink.Command() { Id = id }, CancellationToken.None);

            // Asssert
            Assert.NotNull(result);
            Assert.True(result.Success);
        }

        [Theory]
        [InlineData(-5)]
        [InlineData(0)]
        [InlineData(-100)]
        public async Task CommandValidator_Should_Fail_GivenInvalidData(int id)
        {
            //Arrange
            var ctx = Fixture.CreateContext();
            var command = new DeleteQuickLink.Command() { Id = id };
            var validator = new DeleteQuickLink.CommandValidator();

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
            var handler = new DeleteQuickLink.Handler(new UnitOfWork(ctx));
            var request = new DeleteQuickLink.Command() { Id = 1000000 };

            // Act & Assert
            await Assert.ThrowsAsync<NotFoundException>(() => handler.Handle(request, CancellationToken.None));
        }
    }
}
