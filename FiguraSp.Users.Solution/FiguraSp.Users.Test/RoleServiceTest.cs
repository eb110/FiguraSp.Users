using FiguraSp.Users.Service.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

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
            IList<string> roles = new List<string>();

            mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(user)!);

            mockUserManager.Setup(x => x.GetRolesAsync(It.IsAny<IdentityUser>()))
            .Returns(Task.FromResult(roles));
        }

            #endregion

            #region Test Methods

            [TestMethod]
        public async Task GetUserRoles()
        {
            IList<string>? result = await roleService.GetUserRoles(userEmail);

            Assert.IsNotNull(result);
        }

        #endregion
    }
}
