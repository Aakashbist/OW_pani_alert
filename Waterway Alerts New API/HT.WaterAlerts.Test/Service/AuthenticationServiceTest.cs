using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

namespace HT.WaterAlerts.Test.Service
{
    public class AuthenticationServiceTest
    {
        private const string _email = "admin@hydro.com.au";
        private const string _key = "TokenExpireIn";
        private const string _securedKey = "SecuredKey";
        private const string _password = "admin1234!@#";
        private readonly Mock<ILogger<AuthenticationService>> _logger;
        private readonly IFixture _fixture;
        private readonly Mock<IConfiguration> _configuration;
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;

        public AuthenticationServiceTest()
        {
            _fixture = new Fixture();
            _fixture.Behaviors.OfType<ThrowingRecursionBehavior>().ToList().ForEach(b => _fixture.Behaviors.Remove(b));
            _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
            _logger = new Mock<ILogger<AuthenticationService>>();
            var userStoreMock = Mock.Of<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(userStoreMock, null, null, null, null, null, null, null, null);
            _configuration = new Mock<IConfiguration>();
        }

        [Fact]
        public async void SignIn_GivenValidEmailAndPassword_ShouldReturnValidUser()
        {
            var applicationUser = _fixture.Create<ApplicationUser>();
            applicationUser.Status = true;
            var signInResponseDTO = _fixture.Create<SignInResponseDTO>();
            IList<string> userRoles = new List<string>(){"admin"};

            _configuration.Setup(x => x.GetSection(_key).Value).Returns("7200");
            _configuration.Setup(x => x.GetSection(_securedKey).Value).Returns(GetFakeHash("samplestring"));
            _mockUserManager.Setup(userManager => userManager.FindByEmailAsync(It.IsAny<string>()))
                     .ReturnsAsync(applicationUser);
            _mockUserManager.Setup(userManager => userManager.IsEmailConfirmedAsync(applicationUser))
                  .ReturnsAsync(true);
            _mockUserManager.Setup(userManager => userManager.CheckPasswordAsync(applicationUser, It.IsAny<string>()))
                  .ReturnsAsync(true);
            _mockUserManager.Setup(userManager => userManager.GetRolesAsync(applicationUser))
                  .Returns(Task.FromResult(userRoles));


            var _sut = new AuthenticationService(_configuration.Object, _mockUserManager.Object, _logger.Object);

            var result = await _sut.SignIn(_email, _password);
            result.Should().NotBeNull();
            result.Email.Should().Be(applicationUser.Email);
            result.Id.Should().Be(applicationUser.Id);
        }

        [Fact]
        public async void SignIn_GivenInValidPassword_ShouldThrowException()
        {
            var applicationUser = _fixture.Create<ApplicationUser>();

            _configuration.Setup(x => x.GetSection(_key).Value).Returns("7200");
            _configuration.Setup(x => x.GetSection(_securedKey).Value).Returns(GetFakeHash("samplestring"));
            _mockUserManager.Setup(userManager => userManager.FindByEmailAsync(It.IsAny<string>()))
                     .ReturnsAsync(applicationUser);
            _mockUserManager.Setup(userManager => userManager.IsEmailConfirmedAsync(applicationUser))
                  .ReturnsAsync(true);
            _mockUserManager.Setup(userManager => userManager.CheckPasswordAsync(applicationUser, It.IsAny<string>()))
                  .ReturnsAsync(false);


            var _sut = new AuthenticationService(_configuration.Object, _mockUserManager.Object, _logger.Object);

            Func<Task> sutMethod = async () => { await _sut.SignIn(_email, _password); };
            await sutMethod.Should().ThrowAsync<Exception>().WithMessage("Invalid Email or Password");

        }

        [Fact]
        public async void SignIn_GivenInValidEmail_ShouldThrowException()
        {
            _configuration.Setup(x => x.GetSection(_key).Value).Returns("7200");
            _configuration.Setup(x => x.GetSection(_securedKey).Value).Returns(GetFakeHash("samplestring"));
            _mockUserManager.Setup(userManager => userManager.FindByEmailAsync(It.IsAny<string>()))
                     .Throws(new Exception("User not found"));

            var _sut = new AuthenticationService(_configuration.Object, _mockUserManager.Object, _logger.Object);
            Func<Task> result = async () => { await _sut.SignIn(_email, _password); };
            await result.Should().ThrowAsync<Exception>().WithMessage("Invalid Email or Password");

        }

        [Fact]
        public async void SignIn_GivenInActiveEmail_ShouldThrowException()
        {
            var applicationUser = _fixture.Create<ApplicationUser>();
            _configuration.Setup(x => x.GetSection(_key).Value).Returns("7200");
            _configuration.Setup(x => x.GetSection(_securedKey).Value).Returns(GetFakeHash("samplestring"));
            _mockUserManager.Setup(userManager => userManager.FindByEmailAsync(It.IsAny<string>()))
                    .ReturnsAsync(applicationUser);
            _mockUserManager.Setup(userManager => userManager.IsEmailConfirmedAsync(applicationUser))
                  .ReturnsAsync(false);

            var _sut = new AuthenticationService(_configuration.Object, _mockUserManager.Object, _logger.Object);
            Func<Task> result = async () => { await _sut.SignIn(_email, _password); };
            await result.Should().ThrowAsync<Exception>().WithMessage("Invalid Email or Password");

        }

        [Fact]
        public async void SignUp_GivenValidSignUpDTO_ShouldReturnsTrue()
        {
            var signUpUser = _fixture.Create<SignUpDTO>();
            _configuration.Setup(x => x.GetSection(_key).Value).Returns("7200");
            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                    .ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(userManager => userManager.AddToRoleAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                    .ReturnsAsync(IdentityResult.Success); 

            var _sut = new AuthenticationService(_configuration.Object, _mockUserManager.Object, _logger.Object);
            var result = await _sut.SignUp(signUpUser);

            result.Should().NotBeNull().And.Match<IdentityResult>(_ => _.Succeeded == true);
        }

        [Fact]
        public async void ConfirmEmail_GivenValidEmail_ShouldReturnsTrue()
        {
            var applicationUser = _fixture.Create<ApplicationUser>();
            _configuration.Setup(x => x.GetSection(_key).Value).Returns("7200");
            _mockUserManager.Setup(userManager => userManager.FindByEmailAsync(It.IsAny<string>()))
                            .ReturnsAsync(applicationUser);
            _mockUserManager.Setup(userManager => userManager.ConfirmEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                    .ReturnsAsync(IdentityResult.Success);

            var _sut = new AuthenticationService(_configuration.Object, _mockUserManager.Object, _logger.Object);
            var result = await _sut.ConfirmEmail(_email,It.IsAny<string>());

            result.Should().NotBeNull().And.Match<IdentityResult>(_ => _.Succeeded == true);
        }

        [Fact]
        public async void ConfirmEmail_GivenInValidEmail_ShouldThrowException()
        {
            _configuration.Setup(x => x.GetSection(_key).Value).Returns("7200");
            _mockUserManager.Setup(userManager => userManager.FindByEmailAsync(It.IsAny<string>()))
                            .ReturnsAsync(()=>null);
            _mockUserManager.Setup(userManager => userManager.ConfirmEmailAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>()))
                   .Throws(new ArgumentNullException());

            var _sut = new AuthenticationService(_configuration.Object, _mockUserManager.Object, _logger.Object);
            Func<Task> result = async () => { await _sut.ConfirmEmail(_email, It.IsAny<string>()); };

            await result.Should().ThrowAsync<ArgumentNullException>();
           
        }

        [Fact]
        public async void ResetPassword_GivenInValidEmail_ShouldThrowException()
        {
            _configuration.Setup(x => x.GetSection(_key).Value).Returns("7200");
            _mockUserManager.Setup(userManager => userManager.FindByEmailAsync(It.IsAny<string>()))
                            .ReturnsAsync(() => null);
            _mockUserManager.Setup(userManager => userManager.ResetPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>()))
                   .Throws(new ArgumentNullException());

            var _sut = new AuthenticationService(_configuration.Object, _mockUserManager.Object, _logger.Object);
            Func<Task> result = async () => { await _sut.ResetPassword(_email, It.IsAny<string>(), It.IsAny<string>()); };

            await result.Should().ThrowAsync<ArgumentNullException>();

        }

        [Fact]
        public async void ResetPassword_GivenValidEmail_ShouldReturnsTrue()
        {
            var applicationUser = _fixture.Create<ApplicationUser>();
            _configuration.Setup(x => x.GetSection(_key).Value).Returns("7200");
            _mockUserManager.Setup(userManager => userManager.FindByEmailAsync(It.IsAny<string>()))
                            .ReturnsAsync(applicationUser);
            _mockUserManager.Setup(userManager => userManager.ResetPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>()))
                  .ReturnsAsync(IdentityResult.Success);

            var _sut = new AuthenticationService(_configuration.Object, _mockUserManager.Object, _logger.Object);
            var result = await _sut.ResetPassword(_email, It.IsAny<string>(), It.IsAny<string>());

            result.Should().NotBeNull().And.Match<IdentityResult>(_ => _.Succeeded == true);
        }

        private static string GetFakeHash(string value)
        {
            StringBuilder Sb = new StringBuilder();

            using (var hash = SHA256.Create())
            {
                Encoding enc = Encoding.UTF8;
                byte[] result = hash.ComputeHash(enc.GetBytes(value));

                foreach (byte b in result)
                    Sb.Append(b.ToString("x2"));
            }

            return Sb.ToString();
        }
    }
}
