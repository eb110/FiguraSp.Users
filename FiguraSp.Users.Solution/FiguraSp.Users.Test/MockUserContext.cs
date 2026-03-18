using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace FiguraSp.Users.Test
{
    public class MockUserContext
    {
        public static Mock<DbSet<IdentityRole>> GetMockRoleIdentity()
        {
            var roleIdentityModel = new List<IdentityRole>
            {
                new()
            }.AsQueryable();

            var mockIdentityRole = new Mock<DbSet<IdentityRole>>();

            mockIdentityRole.As<IQueryable<IdentityRole>>().Setup(m => m.Provider).Returns(roleIdentityModel.Provider);
            mockIdentityRole.As<IQueryable<IdentityRole>>().Setup(m => m.Expression).Returns(roleIdentityModel.Expression);
            mockIdentityRole.As<IQueryable<IdentityRole>>().Setup(m => m.ElementType).Returns(roleIdentityModel.ElementType);
            mockIdentityRole.As<IQueryable<IdentityRole>>().Setup(m => m.GetEnumerator()).Returns(roleIdentityModel.GetEnumerator());

            return mockIdentityRole;
        }

        public static Mock<DbSet<IdentityUser>> GetMockUserIdentity()
        {
            var roleIdentityModel = new List<IdentityUser>
            {
                new()
            }.AsQueryable();

            var mockIdentityUser = new Mock<DbSet<IdentityUser>>();

            mockIdentityUser.As<IQueryable<IdentityUser>>().Setup(m => m.Provider).Returns(roleIdentityModel.Provider);
            mockIdentityUser.As<IQueryable<IdentityUser>>().Setup(m => m.Expression).Returns(roleIdentityModel.Expression);
            mockIdentityUser.As<IQueryable<IdentityUser>>().Setup(m => m.ElementType).Returns(roleIdentityModel.ElementType);
            mockIdentityUser.As<IQueryable<IdentityUser>>().Setup(m => m.GetEnumerator()).Returns(roleIdentityModel.GetEnumerator());

            return mockIdentityUser;
        }
    }
}
