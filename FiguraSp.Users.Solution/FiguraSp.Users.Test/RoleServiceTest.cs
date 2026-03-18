using FiguraSp.Users.Model.DTOs.Responses;
using FiguraSp.Users.Service.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Data;
using static Microsoft.ApplicationInsights.MetricDimensionNames.TelemetryContext;

namespace FiguraSp.Users.Test
{
    [TestClass]
    public class RoleServiceTest
    {
        #region Variables

        private RoleService roleService;
        private Mock<UserManager<IdentityUser>> mockUserManager;
        private Mock<RoleManager<IdentityRole>> mockRoleManager;

        private string userEmail;

        #endregion

        #region Controller

        public RoleServiceTest()
        {
            userEmail = "a2@op.pl";
            mockUserManager = new Mock<UserManager<IdentityUser>>(Mock.Of<IUserStore<IdentityUser>>(), null!, null!, null!, null!, null!, null!, null!, null!);
            mockRoleManager = new Mock<RoleManager<IdentityRole>>(Mock.Of<IRoleStore<IdentityRole>>(), null!, null!, null!, null!);
            roleService = new RoleService(mockUserManager.Object, mockRoleManager.Object);
        }

        #endregion

        #region Initialize

        [TestInitialize]
        public void Initialize()
        {
            IdentityUser user = new IdentityUser();
            IdentityRole role = new IdentityRole();
            IdentityResult identityResult = IdentityResult.Success;
            IList<string> roles = new List<string>();

            mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(user)!);        

            mockUserManager.Setup(x => x.GetRolesAsync(It.IsAny<IdentityUser>()))
                .Returns(Task.FromResult(roles));

            mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .Returns(Task.FromResult(identityResult));

            mockRoleManager.Setup(x => x.RoleExistsAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(true));

            mockRoleManager.Setup(x => x.CreateAsync(It.IsAny<IdentityRole>()))
                .Returns(Task.FromResult(identityResult));
        }

        #endregion

        #region Test Methods

        [TestMethod]
        public async Task AddUserToRoleSuccess()
        {
            RoleResponseDto result = await roleService.AddUserToRole("", "");
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
        }

        [TestMethod]
        public async Task AddUserToRoleFailed()
        {
            IdentityResult identityResult = IdentityResult.Failed();
            RoleResponseDto result = await roleService.AddUserToRole("", "");
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
        }

        [TestMethod]
        public async Task AddRoleReturnOne()
        {
            mockRoleManager.Setup(x => x.RoleExistsAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(false));
            RoleResponseDto result = await roleService.AddRole("test");
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
        }

        [TestMethod]
        public async Task AddRoleFails()
        {
            IdentityResult identityResult = IdentityResult.Failed();
            RoleResponseDto result = await roleService.AddRole("test");
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Success);
        }

        [TestMethod]
        public async Task GetUserRolesReturnsList()
        {
            IList<string>? result = await roleService.GetUserRoles(userEmail);
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetUserRolesDoesNotReturnList()
        {
            mockUserManager.Setup(x => x.GetRolesAsync(It.IsAny<IdentityUser>()));
            IList<string>? result = await roleService.GetUserRoles(userEmail);
            Assert.IsNull(result);
        }

        #endregion
    }
}
