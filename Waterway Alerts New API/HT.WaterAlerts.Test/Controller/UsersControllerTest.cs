using AutoFixture.Xunit2;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using System.Security.Claims;
using System.Security.Principal;

namespace HT.WaterAlerts.Test.Controller
{
    public class UsersControllerTest
    {

        private readonly UsersController _controller;
        public UsersControllerTest()
        {

        }

        [Theory, AutoMoqData]
        public void Get_GivenValidUserId_ShouldReturnHttpOK(Guid Id,
                                                        UsersDTO user,
                                                        [Frozen] Mock<IUserService> mockService,
                                                        [Greedy] UsersController sut)
        {
            var identity = new GenericIdentity(Id.ToString(), "Name");
            var contextUser = new ClaimsPrincipal(identity); //add claims as needed

            //...then set user and other required properties on the httpContext as needed
            var httpContext = new DefaultHttpContext()
            {
                User = contextUser
            };

            //Controller needs a controller context to access HttpContext
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };

            //assign context to controller
            var controller = new UsersController(mockService.Object)
            {
                ControllerContext = controllerContext,
            };

            mockService.Setup(x => x.GetUser(It.IsAny<Guid>())).Returns(user);

            var actionResult = controller.Get(Id);
            var response = actionResult as OkObjectResult;
            var result = response?.Value as UsersDTO;

            response.Should().NotBeNull();
            response?.StatusCode.Should().Be(200);
            result?.Id.Should().Be(user.Id);
        }

        [Theory, AutoMoqData]
        public void Get_GivenInvalidUserId_ShouldReturnNotFoundIFNotAdmin(Guid Id,[Frozen] Mock<IUserService> mockService)
        {
            var identity = new GenericIdentity(Id.ToString(), "Name");
            var contextUser = new ClaimsPrincipal(identity); //add claims as needed

            //...then set user and other required properties on the httpContext as needed
            var httpContext = new DefaultHttpContext()
            {
                User = contextUser
            };

            //Controller needs a controller context to access HttpContext
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };

            //assign context to controller
            var controller = new UsersController(mockService.Object)
            {
                ControllerContext = controllerContext,
            };

            mockService.Setup(x => x.GetUser(It.IsAny<Guid>())).Returns(() => null);

            var actionResult = controller.Get(Id);
            var response = actionResult as NotFoundObjectResult;

            response.Should().NotBeNull();
            response?.StatusCode.Should().Be(404);
        }

        [Theory, AutoMoqData]
        public void Get__GivenValidDataTableResponseDTO_ShouldReturnHttpOKWithListOfDataTableRequestDTO(DataTableResponseDTO dataTableResponse,
                                                                            DataTableRequestDTO request,
                                                                            [Frozen] Mock<IUserService> mockService,
                                                                            [Greedy] UsersController sut)
        {
            mockService.Setup(x => x.GetUsers(It.IsAny<DataTableRequestDTO>())).Returns(dataTableResponse);

            var actionResult = sut.Get(request);
            var response = actionResult as OkObjectResult;
            var result = response?.Value as DataTableResponseDTO;

            response.Should().NotBeNull();
            response?.StatusCode.Should().Be(200);
            result?.Data.Should().NotBeNull();
        }

        [Theory, AutoMoqData]
        public void Get__GivenInValidDataTableResponseDTO_ShouldReturnHttpNotFound(DataTableRequestDTO request,
                                                        DataTableResponseDTO dataTableResponse,
                                                        [Frozen] Mock<IUserService> mockService,
                                                        [Greedy] UsersController sut)
        {
            dataTableResponse.Data = new object[0];
            mockService.Setup(x => x.GetUsers(It.IsAny<DataTableRequestDTO>())).Returns(dataTableResponse);

            var actionResult = sut.Get(request);
            var response = actionResult as NotFoundObjectResult;

            response.Should().NotBeNull();
            response?.StatusCode.Should().Be(404);
        }

        [Theory, AutoMoqData]
        public async void Post_GivenValidUserDto_ShouldReturnHttpOk(UsersDTO user,
                                                      [Frozen] Mock<IUserService> mockService,
                                                      [Greedy] UsersController sut)
        {
            mockService.Setup(x => x.CreateUser(It.IsAny<UsersDTO>()).Result).Returns(IdentityResult.Success);

            var actionResult = await sut.Post(user) as OkObjectResult;

            actionResult?.StatusCode.Should().Be(200);
        }

        [Theory, AutoMoqData]
        public async void Post_GivenInvalidUserDto_ShouldReturnHttpBadRequest(UsersDTO user,
                                                     [Frozen] Mock<IUserService> mockService,
                                                     [Greedy] UsersController sut)
        {
            mockService.Setup(x => x.CreateUser(It.IsAny<UsersDTO>()).Result).Returns(IdentityResult.Failed(new IdentityError[]
                     {
                      new IdentityError{Code="HT400"}
                     }));

            var actionResult = await sut.Post(user);
            var response = actionResult as BadRequestObjectResult;
            var result = response?.Value as ErrorResponseDTO;
            var errors = result?.Error as List<IdentityError>;

            response.Should().NotBeNull();
            errors?[0].Code.Should().Be("HT400");
            response?.StatusCode.Should().Be(400);
        }

        [Theory, AutoMoqData]
        public async void Put_GivenValidUserDto_ShouldReturnHttpOk(UsersDTO user,
                                                     [Frozen] Mock<IUserService> mockService,
                                                     [Greedy] UsersController sut)
        {
            mockService.Setup(x => x.UpdateUser(It.IsAny<UsersDTO>()).Result).Returns(IdentityResult.Success);
            var actionResult = await sut.Put(user) as OkObjectResult;

            actionResult?.StatusCode.Should().Be(200);
        }

        [Theory, AutoMoqData]
        public async void Put_GivenInvalidUserDto_ShouldReturnHttpBadRequest(UsersDTO user,
                                                     [Frozen] Mock<IUserService> mockService,
                                                     [Greedy] UsersController sut)
        {
            mockService.Setup(x => x.UpdateUser(It.IsAny<UsersDTO>()).Result).Returns(IdentityResult.Failed(new IdentityError[]
                     {
                      new IdentityError{Code="HT400"}
                     }));

            var actionResult = await sut.Put(user);
            var response = actionResult as BadRequestObjectResult;
            var result = response?.Value as ErrorResponseDTO;
            var errors = result?.Error as List<IdentityError>;

            response.Should().NotBeNull();
            errors?[0].Code.Should().Be("HT400");
            response?.StatusCode.Should().Be(400);
        }

        [Theory, AutoMoqData]
        public async void Delete_GivenValidUserId_ShouldReturnHttpOk(Guid Id,
                                                    [Frozen] Mock<IUserService> mockService,
                                                    [Greedy] UsersController sut)
        {
            mockService.Setup(x => x.DeleteUser(It.IsAny<Guid>()).Result).Returns(IdentityResult.Success);
            var actionResult = await sut.Delete(Id) as OkObjectResult;

            actionResult?.StatusCode.Should().Be(200);
        }

        [Theory, AutoMoqData]
        public async void Delete_GivenInvalidUserId_ShouldReturnHttpBadRequest(Guid Id,
                                                     [Frozen] Mock<IUserService> mockService,
                                                     [Greedy] UsersController sut)
        {
            mockService.Setup(x => x.DeleteUser(It.IsAny<Guid>()).Result).Returns(IdentityResult.Failed(new IdentityError[]
                     {
                      new IdentityError{Code="HT400"}
                     }));

            var actionResult = await sut.Delete(Id);
            var response = actionResult as BadRequestObjectResult;
            var result = response?.Value as ErrorResponseDTO;
            var errors = result?.Error as List<IdentityError>;

            response.Should().NotBeNull();
            errors?[0].Code.Should().Be("HT400");
            response?.StatusCode.Should().Be(400);
        }

        [Theory, AutoMoqData]
        public async void Patch_GivenValidUserId_ShouldReturnHttpOk(Guid Id,
                                                    JsonPatchDocument patchUser,
                                                    [Frozen] Mock<IUserService> mockService)
        {
            var identity = new GenericIdentity(Id.ToString(), "Name");
            var contextUser = new ClaimsPrincipal(identity); //add claims as needed

            //...then set user and other required properties on the httpContext as needed
            var httpContext = new DefaultHttpContext()
            {
                User = contextUser
            };

            //Controller needs a controller context to access HttpContext
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };

            //assign context to controller
            var controller = new UsersController(mockService.Object)
            {
                ControllerContext = controllerContext,
            };
            mockService.Setup(x => x.UpdateUserPartial(It.IsAny<Guid>(), It.IsAny<JsonPatchDocument>(), It.IsAny<bool>()).Result).Returns(IdentityResult.Success);
            var actionResult = await controller.Patch(Id, patchUser) as OkObjectResult;

            actionResult?.StatusCode.Should().Be(200);
        }

        [Theory, AutoMoqData]
        public async void Patch_GivenInvalidUserId_ShouldReturnHttpBadRequest(Guid Id,
                                                    JsonPatchDocument patchUser,
                                                     [Frozen] Mock<IUserService> mockService)
        {
            var identity = new GenericIdentity(Id.ToString(), "Name");
            var contextUser = new ClaimsPrincipal(identity); //add claims as needed

            //...then set user and other required properties on the httpContext as needed
            var httpContext = new DefaultHttpContext()
            {
                User = contextUser
            };

            //Controller needs a controller context to access HttpContext
            var controllerContext = new ControllerContext()
            {
                HttpContext = httpContext,
            };

            //assign context to controller
            var controller = new UsersController(mockService.Object)
            {
                ControllerContext = controllerContext,
            };
            mockService.Setup(x => x.UpdateUserPartial(It.IsAny<Guid>(), It.IsAny<JsonPatchDocument>(), It.IsAny<bool>()).Result).Returns(IdentityResult.Failed(new IdentityError[]
                     {
                      new IdentityError{Code="HT400"}
                     }));

            var actionResult = await controller.Patch(Id, patchUser);
            var response = actionResult as BadRequestObjectResult;
            var result = response?.Value as ErrorResponseDTO;
            var errors = result?.Error as List<IdentityError>;

            response.Should().NotBeNull();
            errors?[0].Code.Should().Be("HT400");
            response?.StatusCode.Should().Be(400);
        }
    }
}
