using FiguraSp.Users.Model.Data;
using FiguraSp.Users.Model.DTOs.Requests;
using FiguraSp.Users.Model.DTOs.Responses;
using FiguraSp.Users.Model.Entity;
using FiguraSp.Users.Service.Configuration;
using FiguraSp.Users.Service.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Security.Claims;

namespace FiguraSp.Users.Test
{
    [TestClass]
    public class UserServiceTest
    {
        #region Variables

        private readonly UserService userService;
        private readonly Mock<UserManager<IdentityUser>> mockUserManager;
        private readonly Mock<UsersDbContext> mockUserContext;
        private readonly Mock<DbSet<IdentityUser>> mockIdentityUser;
        private readonly Mock<DbSet<RefreshToken>> mockRefreshToken;
        private readonly IdentityUser user;
        private readonly JwtConfiguration jwtConfig;

        #endregion

        #region Controller

        public UserServiceTest()
        {
            DbContextOptions<UsersDbContext> options = new();
            mockUserManager = new Mock<UserManager<IdentityUser>>(Mock.Of<IUserStore<IdentityUser>>(), null!, null!, null!, null!, null!, null!, null!, null!);
            mockUserContext = new Mock<UsersDbContext>(options);
            userService = new UserService(mockUserManager.Object, mockUserContext.Object);
            mockIdentityUser = MockUserContext.GetMockUserIdentity();
            mockRefreshToken = MockUserContext.GetMockRefreshToken();
            user = new()
            {
                Email = "a@op.pl",
                Id = "123"
            };
            jwtConfig = new()
            {
                SecretKey = "12345678912345671234567891234567!abcA"
            };
        }

        #endregion

        #region Initialize

        [TestInitialize]
        public void Initialize()
        {
            IdentityRole role = new();
            Claim claim = new("test", "test");
            IList<string> roles = ["User"];
            IList<Claim> claims = [claim];

            mockUserContext.Setup(x => x.Users).Returns(mockIdentityUser.Object);
            mockUserContext.Setup(x => x.RefreshTokens).Returns(mockRefreshToken.Object);

            mockUserContext.Setup(x => x.GetEntitiesToListAsync(It.IsAny<IQueryable<IdentityUser>>()))
                .Returns(Task.FromResult(new List<IdentityUser> { user }));

            mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(user)!);

            mockUserManager.Setup(x => x.CheckPasswordAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .Returns(Task.FromResult(true));

            mockUserManager.Setup(x => x.GetRolesAsync(It.IsAny<IdentityUser>()))
                .Returns(Task.FromResult(roles));

            mockUserManager.Setup(x => x.GetClaimsAsync(It.IsAny<IdentityUser>()))
                .Returns(Task.FromResult(claims));

        }

        #endregion

        #region

        [TestMethod]
        public async Task GetUserDetails_UserExist_ReturnUserResponseDtoWithSuccessTrue()
        {
            UserResponseDto result = await userService.GetUserDetails("");
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
        }

        [TestMethod]
        public async Task GetUserDetails_UserExist_ReturnUserResponseDtoWithSuccessFalse()
        {
            mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>()));

            UserResponseDto result = await userService.GetUserDetails("");
            Assert.IsNotNull(result);
            Assert.IsTrue(!result.Success);
        }

        [TestMethod]
        public async Task GetUsers_ReturnListOfUsers()
        {
            List<IdentityUser> result = await userService.GetUsers();
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, 1);
        }

        [TestMethod]
        public async Task GetClaims_ReturnListOfClaims()
        {
            List<Claim> result = await userService.GetAllValidClaims(user);
            Assert.IsNotNull(result);
            Assert.AreEqual(result.Count, 4);
        }

        [TestMethod]
        public async Task GenerateJwtToken_ReturnsOne()
        {
            UserResponseDto result = await userService.GenerateJwtToken(user, jwtConfig);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
        }

        [TestMethod]
        public async Task LoginUser_UserExist_ReturnUserResponseDtoWithSuccessTrue()
        {
            UserLoginRequestDto userDto = new() { Email = "a@op.pl", Password = "password" };
            UserResponseDto result = await userService.LoginUser(userDto, jwtConfig);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
        }

        #endregion
    }
}
