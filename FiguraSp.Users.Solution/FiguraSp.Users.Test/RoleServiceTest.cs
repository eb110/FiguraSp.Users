using FiguraSp.Users.Model.Data;
using FiguraSp.Users.Model.DTOs.Responses;
using FiguraSp.Users.Service.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace FiguraSp.Users.Test
{
    [TestClass]
    public class RoleServiceTest
    {
        #region Variables

        private readonly RoleService roleService;
        private readonly Mock<UserManager<IdentityUser>> mockUserManager;
        private readonly Mock<RoleManager<IdentityRole>> mockRoleManager;
        private readonly Mock<DbSet<IdentityRole>> mockIdentityRole;
        private readonly Mock<DbSet<IdentityUser>> mockIdentityUser;
        private readonly Mock<UsersDbContext> mockUserContext;

        private readonly string userEmail;

        #endregion

        #region Controller

        public RoleServiceTest()
        {
            DbContextOptions<UsersDbContext> options = new();  
            userEmail = "a2@op.pl";
            mockUserManager = new Mock<UserManager<IdentityUser>>(Mock.Of<IUserStore<IdentityUser>>(), null!, null!, null!, null!, null!, null!, null!, null!);
            mockRoleManager = new Mock<RoleManager<IdentityRole>>(Mock.Of<IRoleStore<IdentityRole>>(), null!, null!, null!, null!);
            mockUserContext = new Mock<UsersDbContext>(options);
            roleService = new RoleService(mockUserManager.Object, mockRoleManager.Object, mockUserContext.Object);
            mockIdentityRole = MockUserContext.GetMockRoleIdentity();
            mockIdentityUser = MockUserContext.GetMockUserIdentity();
        }

        #endregion

        #region Initialize

        [TestInitialize]
        public void Initialize()
        {
            IdentityUser user = new();
            IdentityRole role = new();
            IdentityResult identityResult = IdentityResult.Success;
            IList<string> roles = [];

            mockUserContext.Setup(x => x.Roles).Returns(mockIdentityRole.Object);

            mockUserContext.Setup(x => x.Users).Returns(mockIdentityUser.Object);

            mockUserContext.Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<IQueryable<IdentityRole>>()))
                .Returns(Task.FromResult(role));

            mockUserContext.Setup(x => x.GetEntitiesToListAsync(It.IsAny<IQueryable<IdentityRole>>()))
                .Returns(Task.FromResult(new List<IdentityRole> { role }));

            mockUserContext.Setup(x => x.GetEntitiesToListAsync(It.IsAny<IQueryable<IdentityUser>>()))
                .Returns(Task.FromResult(new List<IdentityUser> { user }));

            mockUserManager.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(user)!);        

            mockUserManager.Setup(x => x.GetRolesAsync(It.IsAny<IdentityUser>()))
                .Returns(Task.FromResult(roles));

            mockUserManager.Setup(x => x.RemoveFromRoleAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .Returns(Task.FromResult(identityResult));

            mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .Returns(Task.FromResult(identityResult));

            mockRoleManager.Setup(x => x.RoleExistsAsync(It.IsAny<string>()))
                .Returns(Task.FromResult(true));

            mockRoleManager.Setup(x => x.CreateAsync(It.IsAny<IdentityRole>()))
                .Returns(Task.FromResult(identityResult));

            mockRoleManager.Setup(x => x.DeleteAsync(It.IsAny<IdentityRole>()))
                .Returns(Task.FromResult(identityResult));

        }

        #endregion

        #region Test Methods

        [TestMethod]
        public async Task AddUserRoleToRolesTriggersAddRoleAsync()
        {
            IdentityUser user = new();
            await roleService.AddUserRoleToUsers();
            mockUserManager.Verify(x => x.AddToRoleAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Once());
        }

        [TestMethod]
        public async Task RemoveUserFromRoleSuccess()
        {
            RoleResponseDto result = await roleService.RemoveUserFromRole("", "");
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
        }

        [TestMethod]
        public async Task RemoveUserFromRoleFails()
        {
            IdentityResult identityResult = IdentityResult.Failed();
            mockUserManager.Setup(x => x.RemoveFromRoleAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .Returns(Task.FromResult(identityResult));
            RoleResponseDto result = await roleService.RemoveUserFromRole("", "");
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Success);
        }

        [TestMethod]
        public async Task GetRolesSuccess()
        {
            List<IdentityRole> result = await roleService.GetRoles();
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Count == 1);
        }

        [TestMethod]
        public async Task GetRoleSuccess()
        {
            RoleResponseDto result = await roleService.GetRole("");
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
        }

        [TestMethod]
        public async Task GetRoleFails()
        {
            mockUserContext.Setup(x => x.GetFirstOrDefaultAsync(It.IsAny<IQueryable<IdentityRole>>()));
            RoleResponseDto result = await roleService.GetRole("");
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Success);
        }

        [TestMethod]
        public async Task DeleteRoleSuccess()
        {
            RoleResponseDto result = await roleService.DeleteRole("");
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
        }

        [TestMethod]
        public async Task DeleteRoleFails()
        {
            IdentityResult identityResult = IdentityResult.Failed();
            mockRoleManager.Setup(x => x.DeleteAsync(It.IsAny<IdentityRole>()))
                .Returns(Task.FromResult(identityResult));
            RoleResponseDto result = await roleService.DeleteRole("");
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Success);
        }

        [TestMethod]
        public async Task AddUserToRoleSuccess()
        {
            RoleResponseDto result = await roleService.AddUserToRole("", "");
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Success);
        }

        [TestMethod]
        public async Task AddUserToRoleFails()
        {
            IdentityResult identityResult = IdentityResult.Failed();
            mockUserManager.Setup(x => x.AddToRoleAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
              .Returns(Task.FromResult(identityResult));
            RoleResponseDto result = await roleService.AddUserToRole("", "");
            Assert.IsNotNull(result);
            Assert.IsFalse(result.Success);
        }

        [TestMethod]
        public async Task AddRoleReturnsOne()
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
