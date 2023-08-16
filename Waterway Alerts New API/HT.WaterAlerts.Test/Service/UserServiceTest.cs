using HT.WaterAlerts.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Microsoft.Data.SqlClient;
using System.Linq.Expressions;

namespace HT.WaterAlerts.Test.Service
{
    public class UserServiceTest
    {

        private readonly Mock<IUnitOfWork> _mockUow;
        private readonly Mock<IUserStore<ApplicationUser>> _userStoreMock;
        private readonly Mock<UserManager<ApplicationUser>> _mockUserManager;
        private readonly UserService _sut;
        private IList<string> userRoles = new List<string>(){
                "admin"
            };
        private IdentityError[] error = new IdentityError[]
                    {
                      new IdentityError{Description="Passwords must be at least 8 characters."},
                      new IdentityError{Description="Passwords must have at least one non alphanumeric character."},
                      new IdentityError{Description="Passwords must have at least one digit ('0'-'9')."},
                    };

        public UserServiceTest()
        {
            _mockUow = new Mock<IUnitOfWork>();
            _userStoreMock = new Mock<IUserStore<ApplicationUser>>();
            _mockUserManager = new Mock<UserManager<ApplicationUser>>(_userStoreMock.Object, null, null, null, null, null, null, null, null);
            _sut = new UserService(_mockUow.Object, _mockUserManager.Object);
        }

        [Theory, AutoMoqData]
        public void GetUsers_ShouldReturnListOfUsers([CollectionSize(6)] List<ApplicationUser> users)
        {
            _mockUow.Setup(x => x.GetRepository<ApplicationUser>().GetAll()).Returns(users.AsQueryable());

            var result = _sut.GetUsers().ToList();

            result.Should().NotBeNull();
            result.Count.Should().Be(6);
        }

        [Theory, AutoMoqData]
        public void GetUsers_GivenValidRequest_ShouldReturnDataTableResponseDTO(DataTableRequestDTO dataTablerequestDTO,
                                                                    [CollectionSize(20)] List<ApplicationUser> levels)
        {
            dataTablerequestDTO.SearchColumn = String.Empty;
            dataTablerequestDTO.OrderColumn = String.Empty;
            dataTablerequestDTO.PageNumber = 1;
            dataTablerequestDTO.PageSize = 10;
            var param = ServiceHelper.GetDataTableParams(dataTablerequestDTO);
            var criteria = ServiceHelper.GetFilterPredicate<ApplicationUser>(param.Filter);
            var orderBy = ServiceHelper.GetOrderPredicate<ApplicationUser>(param.OrderColumn);

            _mockUow.Setup(X => X.GetRepository<ApplicationUser>().Get(It.IsAny<Expression<Func<ApplicationUser, bool>>>(),
                                                                    It.IsAny<Expression<Func<ApplicationUser, object>>>(),
                                                                    It.IsAny<SortOrder>(),
                                                                    It.IsAny<int>(),
                                                                    It.IsAny<int>()))
                                                                    .Returns(levels.AsQueryable());
            _mockUow.Setup(X => X.GetRepository<ApplicationUser>().Count(It.IsAny<Expression<Func<ApplicationUser, bool>>>())).Returns(20);

            var result = _sut.GetUsers(dataTablerequestDTO);

            result.Should().NotBeNull();
            result.TotalPage.Should().Be(2);
            result.CurrentPage.Should().Be(1);
            result.TotalCount.Should().Be(20);
            result.Data.Should().NotBeNullOrEmpty();
        }

        [Theory, AutoMoqData]
        public async void GetUser_GivenValidUsersDTO_ShouldReturnTrue(UsersDTO userDto, ApplicationUser applicationUser)
        {
            _mockUow.Setup(x => x.GetRepository<ApplicationUser>().GetById(It.IsAny<Guid>())).Returns(applicationUser);
            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(x => x.AddToRolesAsync(It.IsAny<ApplicationUser>(), It.IsAny<List<string>>())).ReturnsAsync(IdentityResult.Success);

            var result = await _sut.CreateUser(userDto);

            result.Should().NotBeNull().And.Match<IdentityResult>(_ => _.Succeeded == true);
        }

        [Theory, AutoMoqData]
        public async void GetUser_GivenInvalidUsersDTO_ShouldReturnFalse(UsersDTO userDto, ApplicationUser applicationUser)
        {
            var error = new IdentityError[] { new IdentityError { Code = "CreateUserError" } };

            _mockUow.Setup(x => x.GetRepository<ApplicationUser>().GetById(It.IsAny<Guid>())).Returns(applicationUser);
            _mockUserManager.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Failed(error));

            var result = await _sut.CreateUser(userDto);
            var errors = result?.Errors as List<IdentityError>;

            errors?[0].Code.Should().Be("CreateUserError");
            result.Should().NotBeNull().And.Match<IdentityResult>(_ => _.Succeeded == false);
        }

        [Fact]
        public async void DeleteUser_GivenInvalidUserId_ShouldReturnFalse()
        {
            var error = new IdentityError[] { new IdentityError { Code = "DeleteUserError", Description = "User is not found." } };
            _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(() => null);

            var result = await _sut.DeleteUser(Guid.NewGuid());
            var errors = result?.Errors as List<IdentityError>;

            errors?[0].Code.Should().Be("DeleteUserError");
            errors?[0].Description.Should().Be("User is not found.");
            result.Should().NotBeNull().And.Match<IdentityResult>(_ => _.Succeeded == false);
        }

        [Theory, AutoMoqData]
        public async void DeleteUser_GivenValidUserId_ShouldReturnTrue(ApplicationUser applicationUser)
        {
            _mockUserManager.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(applicationUser);
            _mockUserManager.Setup(x => x.DeleteAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);
            var result = await _sut.DeleteUser(Guid.NewGuid());

            result.Should().NotBeNull().And.Match<IdentityResult>(_ => _.Succeeded == true);
        }

        [Theory, AutoMoqData]
        public async void UpdateUser_GivenValidUserDto_ShouldReturnTrue(UsersDTO userDto,
                                                                             Mock<IPasswordValidator<ApplicationUser>> validator,
                                                                             ApplicationUser applicationUser)
        {

            _mockUow.Setup(x => x.GetRepository<ApplicationUser>().GetById(It.IsAny<Guid>())).Returns(applicationUser);
            _mockUserManager.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Object.PasswordValidators.Add(validator.Object);
            validator.Setup(x => x.ValidateAsync(_mockUserManager.Object, null, It.IsAny<string>()).Result).Returns(IdentityResult.Success);
            _mockUserManager.Setup(x => x.GeneratePasswordResetTokenAsync(It.IsAny<ApplicationUser>())).ReturnsAsync("ADSADADAWD");
            _mockUserManager.Setup(x => x.ResetPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Setup(userManager => userManager.GetRolesAsync(applicationUser)).Returns(Task.FromResult(userRoles));
            _mockUserManager.Setup(userManager => userManager.AddToRolesAsync(It.IsAny<ApplicationUser>(), userDto.Roles)).ReturnsAsync(IdentityResult.Success);

            var result = await _sut.UpdateUser(userDto);

            result.Should().NotBeNull().And.Match<IdentityResult>(_ => _.Succeeded == true);
        }

        [Theory, AutoMoqData]
        public async void UpdateUser_GivenInvalidPassword_ShouldReturnFalse(UsersDTO userDto,
                                                                             Mock<IPasswordValidator<ApplicationUser>> validator,
                                                                             ApplicationUser applicationUser)
        {

            _mockUow.Setup(x => x.GetRepository<ApplicationUser>().GetById(It.IsAny<Guid>())).Returns(applicationUser);
            _mockUserManager.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Object.PasswordValidators.Add(validator.Object);
            validator.Setup(x => x.ValidateAsync(_mockUserManager.Object, null, "admim").Result).Returns(IdentityResult.Failed(error));

            var result = await _sut.UpdateUser(userDto);
            var errors = result?.Errors as List<IdentityError>;

            result.Should().NotBeNull().And.Match<IdentityResult>(_ => _.Succeeded == false);
        }

        [Theory, AutoMoqData]
        public async void UpdateUser_GivenInvalidUserDto_ShouldReturnFalse(UsersDTO userDto, ApplicationUser applicationUser)
        {

            _mockUow.Setup(x => x.GetRepository<ApplicationUser>().GetById(It.IsAny<Guid>())).Returns(applicationUser);
            _mockUserManager.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Failed(new IdentityError[]
                     {
                      new IdentityError{Description="User Not Found"}
                     }));

            var result = await _sut.UpdateUser(userDto);
            var errors = result?.Errors as List<IdentityError>;

            errors?[0].Description.Should().Be("User Not Found");

            result.Should().NotBeNull().And.Match<IdentityResult>(_ => _.Succeeded == false);
        }

        [Theory, AutoMoqData]
        public async void UpdateUserPartial_Given_ValidUserIdForResetPassword_ShouldReturnTrue(Guid Id,
                                                                            JsonPatchDocument patchUser,
                                                                            ApplicationUser applicationUser)
        {
            _mockUow.Setup(x => x.GetRepository<ApplicationUser>().GetById(It.IsAny<Guid>())).Returns(applicationUser);
            _mockUserManager.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);

            var result = await _sut.UpdateUserPartial(Id, patchUser, true);

            result.Should().NotBeNull().And.Match<IdentityResult>(_ => _.Succeeded == true);
        }

        [Theory, AutoMoqData]
        public async void UpdateUserPartial_GivenValidUserIdAndPassword_ShouldReturnTrue(Guid Id,
                                                                           JsonPatchDocument patchUser,
                                                                           Operation operations,
                                                                           Mock<IPasswordValidator<ApplicationUser>> validator,
                                                                           ApplicationUser applicationUser)
        {
            operations.path = "password";
            patchUser.Operations.Add(operations);
            _mockUow.Setup(x => x.GetRepository<ApplicationUser>().GetById(It.IsAny<Guid>())).Returns(applicationUser);
            _mockUserManager.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Object.PasswordValidators.Add(validator.Object);
            validator.Setup(x => x.ValidateAsync(_mockUserManager.Object, null, It.IsAny<string>()).Result).Returns(IdentityResult.Success);
            _mockUserManager.Setup(x => x.GeneratePasswordResetTokenAsync(It.IsAny<ApplicationUser>())).ReturnsAsync("ADSADADAWD");
            _mockUserManager.Setup(x => x.ResetPasswordAsync(It.IsAny<ApplicationUser>(), It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);

            var result = await _sut.UpdateUserPartial(Id, patchUser, true);

            result.Should().NotBeNull().And.Match<IdentityResult>(_ => _.Succeeded == true);
        }

        [Theory, AutoMoqData]
        public async void UpdateUserPartial_GivenInvalidPassword_ShouldReturnFalseWithValidationErrorMessages(Guid Id,
                                                                           JsonPatchDocument patchUser,
                                                                           Operation operations,
                                                                           Mock<IPasswordValidator<ApplicationUser>> validator,
                                                                           ApplicationUser applicationUser)
        {
            operations.path = "password";
            patchUser.Operations.Add(operations);
            _mockUow.Setup(x => x.GetRepository<ApplicationUser>().GetById(It.IsAny<Guid>())).Returns(applicationUser);
            _mockUserManager.Setup(x => x.UpdateAsync(It.IsAny<ApplicationUser>())).ReturnsAsync(IdentityResult.Success);
            _mockUserManager.Object.PasswordValidators.Add(validator.Object);
            validator.Setup(x => x.ValidateAsync(_mockUserManager.Object, null, It.IsAny<string>()).Result).Returns(IdentityResult.Failed(error));

            var result = await _sut.UpdateUserPartial(Id, patchUser, true);
            var errors = result?.Errors as List<IdentityError>;

            errors?[0].Description.Should().Be("Passwords must be at least 8 characters.");
            errors?[1].Description.Should().Be("Passwords must have at least one non alphanumeric character.");
            errors?[2].Description.Should().Be("Passwords must have at least one digit ('0'-'9').");
            result.Should().NotBeNull().And.Match<IdentityResult>(_ => _.Succeeded == false);
        }
    }
}
