using AutoFixture.Xunit2;
using Microsoft.AspNetCore.Identity;

namespace HT.WaterAlerts.Test.Controller
{
    public class AuthenticationControllerTest
    {

        [Theory, AutoMoqData]
        public async void SignIn_GivenValidUserEmailAndPassword_ShouldReturnHttpOk(
             SignInDTO signInUser,
             SignInResponseDTO signInResponseDTO,
             [Frozen] Mock<IAuthenticationService> mockService,
             [Greedy] AuthenticationController sut)
        {
            mockService.Setup(x => x.SignIn(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.FromResult(signInResponseDTO));
            var actionResult = await sut.Signin(signInUser);
            var response = actionResult as OkObjectResult;
            var result = response?.Value as SignInResponseDTO;


            response?.StatusCode.Should().Be(200);
            response.Should().NotBeNull();
            result?.Token.Should().NotBeNull();

        }

        [Theory, AutoMoqData]
        public async void SignIn_GivenInValidEmail_ShouldReturnHttpBadRequest(
            SignInDTO signInUser,
            [Frozen] Mock<IAuthenticationService> mockService,
            [Greedy] AuthenticationController sut)
        {
            mockService.Setup(x => x.SignIn(It.IsAny<string>(), It.IsAny<string>())).Throws(new Exception("User Not Active"));
            var actionResult = await sut.Signin(signInUser);

            var response = actionResult as BadRequestObjectResult;
            var result = response?.Value as ErrorResponseDTO;

            result?.Error.Should().Be("User Not Active");
            response.Should().BeOfType(typeof(BadRequestObjectResult));
        }

        [Theory, AutoMoqData]
        public async void SignUp_GivenEmailAlreadyExist_ShouldReturnHttpBadRequest(
           SignUpDTO signUpDTO,
           [Frozen] Mock<IAuthenticationService> mockService,
           [Greedy] AuthenticationController sut)
        {
            mockService.Setup(x => x.SignUp(It.IsAny<SignUpDTO>())).Throws(new Exception("User Already Active"));
            var actionResult = await sut.SignUp(signUpDTO);

            var response = actionResult as BadRequestObjectResult;
            var result = response?.Value as ErrorResponseDTO;

            result?.Error.Should().Be("User Already Active");
            response.Should().BeOfType(typeof(BadRequestObjectResult));
        }


        [Theory, AutoMoqData]
        public async void SignUp_GivenInvalidPassword_ShouldReturnHttpBadRequestWithValidationErrorMessage(
           SignUpDTO signUpDTO,
           [Frozen] Mock<IAuthenticationService> mockService,
           [Greedy] AuthenticationController sut)
        {

            signUpDTO.Password = "password";
            var error = new IdentityError[]
                     {
                      new IdentityError{Description="Passwords must be at least 8 characters."},
                      new IdentityError{Description="Passwords must have at least one non alphanumeric character."},
                      new IdentityError{Description="Passwords must have at least one digit ('0'-'9')."},


                     };
            mockService.Setup(x => x.SignUp(It.IsAny<SignUpDTO>()).Result).Returns(IdentityResult.Failed(error));
            var actionResult = await sut.SignUp(signUpDTO);

            var response = actionResult as BadRequestObjectResult;
            var result = response?.Value as ErrorResponseDTO;
            var errors = result?.Error as List<IdentityError>;

            errors?[0].Description.Should().Be("Passwords must be at least 8 characters.");
            errors?[1].Description.Should().Be("Passwords must have at least one non alphanumeric character.");
            errors?[2].Description.Should().Be("Passwords must have at least one digit ('0'-'9').");
            response.Should().BeOfType(typeof(BadRequestObjectResult));
        }


        [Theory, AutoMoqData]
        public async void SignUp_GivenValidEmailPassword_ShouldReturnHttpOk(
           SignUpDTO signUpDTO,
           [Frozen] Mock<IAuthenticationService> mockService,
           [Greedy] AuthenticationController sut)
        {
            mockService.Setup(x => x.SignUp(It.IsAny<SignUpDTO>()).Result).Returns(IdentityResult.Success);
            var actionResult = await sut.SignUp(signUpDTO);

            var response = actionResult as OkObjectResult;

            response?.StatusCode.Should().Be(200);
        }

        [Theory, AutoMoqData]
        public async void ConfirmEmail_GivenValidEmail_ShouldReturnHttpOk(
            ConfirmEmailDTO confirmEmail,
            [Frozen] Mock<IAuthenticationService> mockService,
            [Greedy] AuthenticationController sut)
        {
            mockService.Setup(x => x.ConfirmEmail(It.IsAny<string>(), It.IsAny<string>()).Result).Returns(IdentityResult.Success);
            var actionResult = await sut.ConfirmEmail(confirmEmail);

            var response = actionResult as OkObjectResult;

            response?.StatusCode.Should().Be(200);
        }

        [Theory, AutoMoqData]
        public async void ConfirmEmail_GivenInvalidEmail_ShouldReturnHttpBadRequest(
           ConfirmEmailDTO confirmEmail,
           [Frozen] Mock<IAuthenticationService> mockService,
           [Greedy] AuthenticationController sut)
        {
            mockService.Setup(x => x.ConfirmEmail(It.IsAny<string>(), It.IsAny<string>()).Result).Throws(new Exception("User not Found"));
            var actionResult = await sut.ConfirmEmail(confirmEmail);

            var response = actionResult as BadRequestObjectResult;
            var result = response?.Value as ErrorResponseDTO;

            result?.Error.Should().Be("User not Found");
            response.Should().BeOfType(typeof(BadRequestObjectResult));
        }

        [Theory, AutoMoqData]
        public async void ResetPassword_GivenValidEmail_ShouldReturnHttpOk(
            ResetPasswordDTO resetUser,
            [Frozen] Mock<IAuthenticationService> mockService,
            [Greedy] AuthenticationController sut)
        {
            mockService.Setup(x => x.ResetPassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()).Result).Returns(IdentityResult.Success);
            var actionResult = await sut.ResetPassword(resetUser);

            var response = actionResult as OkObjectResult;

            response?.StatusCode.Should().Be(200);
        }

        [Theory, AutoMoqData]
        public async void ResetPassword_GivenInvalidEmail_ShouldReturnHttpBadRequest(
           ResetPasswordDTO resetUser,
           [Frozen] Mock<IAuthenticationService> mockService,
           [Greedy] AuthenticationController sut)
        {
            mockService.Setup(x => x.ResetPassword(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()).Result).Throws(new Exception("User not Found"));
            var actionResult = await sut.ResetPassword(resetUser);

            var response = actionResult as BadRequestObjectResult;
            var result = response?.Value as ErrorResponseDTO;

            result?.Error.Should().Be("User not Found");
            response.Should().BeOfType(typeof(BadRequestObjectResult));
        }
    }
}
