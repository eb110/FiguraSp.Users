using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FiguraSp.Users.Service.Services
{
    public class UserService(UserManager<IdentityUser> userManager) : IUserService
    {
        public async Task<List<IdentityUser>> GetUsers()
        {
            List<IdentityUser> users = await userManager.Users.ToListAsync();

            return users;
        }
    }

    public interface IUserService
    {
        public Task<List<IdentityUser>> GetUsers();
    }
}
