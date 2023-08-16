using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using System.Security.Principal;

namespace HT.WaterAlerts.Test.Controller
{
    public class SubscriptionControllerTest
    {
        private readonly Mock<ISubscriptionService> _mockService;
        private readonly IFixture _fixture;
        private readonly SubscriptionsController _sut;
        private readonly Guid Id = Guid.NewGuid();

        public SubscriptionControllerTest()
        {
            _mockService = new Mock<ISubscriptionService>();
            _fixture = new Fixture();
            var identity = new GenericIdentity(Id.ToString(), "Name");
            var contextUser = new ClaimsPrincipal(identity); 
            var httpContext = new DefaultHttpContext()
            {
                User = contextUser
            };
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };

            _sut = new SubscriptionsController(_mockService.Object)
            {
                ControllerContext = controllerContext,
            };
        }

        [Fact]
        public void Get_GivenValidUserId_ShouldReturnHttpOk()
        {
            //Arrange
            var dto = _fixture.CreateMany<SubscriptionResponseDTO>().ToList();
            _mockService.Setup(s => s.GetSubscriptions(It.IsAny<Guid>())).Returns(dto);

            // Act
            var actionResult = _sut.Get(Id);
            var response = actionResult as OkObjectResult;
            var result = response?.Value as IEnumerable<SubscriptionResponseDTO>;

            //Assert
            result.Should().NotBeNull();
            response?.StatusCode.Should().Be(200);
        }

        [Fact]
        public void Get_GivenValidUserId_ShouldReturnHttpStatusOkWithCorrectNumberOfSubscriptionResponseDTO()
        {
            //Arrange
            var dto = _fixture.CreateMany<SubscriptionResponseDTO>(2).ToList();
            _mockService.Setup(s => s.GetSubscriptions(It.IsAny<Guid>())).Returns(dto);

            // Act
            var actionResult = _sut.Get(Id);
            var response = actionResult as OkObjectResult;
            var result = response?.Value as IEnumerable<SubscriptionResponseDTO>;

            //Assert
            result.Should().NotBeNull();
            result?.Count().Should().Be(2);
        }

        [Fact]
        public void Get_GivenNullUserParameters_ShouldReturnHttpBadRequest()
        {
         
            _mockService.Setup(s => s.GetSubscriptions(It.IsAny<Guid>()))
                .Returns(() => null);

            var actionResult = _sut.Get(Id);
            var response = actionResult as BadRequestObjectResult;
            var result = response?.Value as ErrorResponseDTO;

            result?.Error.Should().Be("Value cannot be null. (Parameter 'source')");
            response.Should().BeOfType(typeof(BadRequestObjectResult));
        }

        [Fact]
        public void Get_GivenInvalidUserId_ShouldReturnHttpNotFound()
        {
            var subscriptionResponseDTOs = new List<SubscriptionResponseDTO>();
            _mockService.Setup(s => s.GetSubscriptions(It.IsAny<Guid>()))
                .Returns(subscriptionResponseDTOs);

            var actionResult = _sut.Get(Id);
            var response = actionResult as NotFoundObjectResult;

            response.Should().BeOfType(typeof(NotFoundObjectResult));
        }

        [Fact]
        public void Post_ActionExecutes_ShouldExecuteOnce()
        {
            var response = _sut.Post(It.IsAny<SubscriptionDTO>()) as OkObjectResult;

            response?.StatusCode.Should().Be(200);
        }


    }
}
