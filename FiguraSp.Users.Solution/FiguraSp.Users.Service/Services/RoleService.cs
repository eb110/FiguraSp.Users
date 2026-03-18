using FiguraSp.Users.Model.DTOs.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FiguraSp.Users.Service.Services
{
    public class RoleService(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager) : IRoleService
    {
        public async Task<RoleResponseDto> AddRole(string roleName)
        {
            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                var roleResult = await roleManager.CreateAsync(new IdentityRole(roleName));
                if (roleResult.Succeeded)
                {
                    return new RoleResponseDto { Success = true, RoleName = roleName };
                }
            }
            return new RoleResponseDto { Success = false, Errors = ["Role already exist"] };
        }

        public async Task<IList<string>?> GetUserRoles(string email)
        {
            var userExist = await userManager.FindByEmailAsync(email);
            if (userExist == null)
            {
                return null;
            }
            var roles = await userManager.GetRolesAsync(userExist);

            return roles;
        }

        public async Task<RoleResponseDto> AddUserToRole(string email, string roleName)
        {
            var userExist = await userManager.FindByEmailAsync(email);
            if (userExist == null)
            {
                return new RoleResponseDto
                {
                    Errors = [$"The user {email} does not exist"]
                };
            }

            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                return new RoleResponseDto
                {
                    Errors = [$"The role {roleName} does not exist"]
                };
            }

            var result = await userManager.AddToRoleAsync(userExist, roleName);
            if (result.Succeeded)
            {
                return new RoleResponseDto { Success = true, RoleName = roleName, UserName = userExist.UserName };
            }

            return new RoleResponseDto
            {
                Errors = [$"Adding new role failed"]
            };
        }

        public async Task<RoleResponseDto> DeleteRole(string roleName)
        {
            var roleExist = await roleManager.Roles.FirstOrDefaultAsync(x => x.Name == roleName);
            if (roleExist == null)
            {
                return new RoleResponseDto
                {
                    Success = false,
                    Errors = [$"Role {roleName} does not exist"]
                };
            }

            var success = await roleManager.DeleteAsync(roleExist);
            if (success != null)
            {
                return new RoleResponseDto
                {
                    Success = true
                };
            }
            return new RoleResponseDto
            {
                Success = false,
                Errors = [$"Role {roleName} was not delted"]
            };
        }

        public async Task<RoleResponseDto> GetRole(string roleName)
        {
            var roleExist = await roleManager.Roles.FirstOrDefaultAsync(x => x.Name == roleName);
            if (roleExist == null)
            {
                return new RoleResponseDto
                {
                    Success = false,
                    Errors = [$"Role {roleName} does not exist"]
                };
            }
            return new RoleResponseDto
            {
                Success = true,
                Errors = [],
                RoleName = roleName
            };
        }

        public async Task<List<IdentityRole>> GetRoles()
        {
            var roles = await roleManager.Roles.ToListAsync();
            return roles;
        }

        public async Task<RoleResponseDto> RemoveUserFromRole(string email, string roleName)
        {
            var userExist = await userManager.FindByEmailAsync(email);
            if (userExist == null)
            {
                return new RoleResponseDto
                {
                    Errors = [$"The user {email} does not exist"]
                };
            }

            var roleExist = await roleManager.RoleExistsAsync(roleName);
            if (!roleExist)
            {
                return new RoleResponseDto
                {
                    Errors = [$"The role {roleName} does not exist"]
                };
            }

            var result = await userManager.RemoveFromRoleAsync(userExist, roleName);

            if (result.Succeeded)
            {
                return new RoleResponseDto { Success = true };
            }

            return new RoleResponseDto
            {
                Errors = [$"Remove of role failed"]
            };
        }

        public async Task AddUserRoleToUsers()
        {
            var listOfUsers = await userManager.Users.ToListAsync();
            foreach (var user in listOfUsers)
            {
                await userManager.AddToRoleAsync(user, "User");
            }
            return;
        }
    }

    public interface IRoleService
    {
        public Task<List<IdentityRole>> GetRoles();
        public Task<RoleResponseDto> AddRole(string roleName);
        public Task<RoleResponseDto> GetRole(string roleName);
        public Task<RoleResponseDto> DeleteRole(string roleName);
        public Task<RoleResponseDto> AddUserToRole(string email, string roleName);
        public Task<IList<string>?> GetUserRoles(string email);
        public Task<RoleResponseDto> RemoveUserFromRole(string email, string roleName);
        public Task AddUserRoleToUsers();
    }
}
