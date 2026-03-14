using FiguraSp.Users.Model.DTOs.Requests;
using FiguraSp.Users.Model.DTOs.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FiguraSp.Users.Service.Services
{
    public class UserService(UserManager<IdentityUser> userManager) : IUserService
    {
        public async Task<UserResponseDto> GetUserDetails(string email)
        {
            var userExist = await userManager.FindByEmailAsync(email);
            if (userExist == null)
            {
                return new UserResponseDto
                {
                    Success = false,
                    Errors = new List<string>() { "user does not exist" }
                };
            }

            return new UserResponseDto
            {
                Username = userExist.UserName,
                Email = userExist.Email,
                Success = true,
                Errors = []
            };
        }

        public async Task<List<IdentityUser>> GetUsers()
        {
            List<IdentityUser> users = await userManager.Users.ToListAsync();

            return users;
        }

        public async Task<UserResponseDto> LoginUser(UserLoginRequestDto userToLogin)
        {
            var userExist = await userManager.FindByEmailAsync(userToLogin.Email);
            if (userExist == null || !await userManager.CheckPasswordAsync(userExist, userToLogin.Password))
            {
                return new UserResponseDto
                {
                    Success = false,
                    Errors = new List<string>() { "wrong login credentials" }
                };
            }
            return new UserResponseDto
            {
                Username = userExist.UserName,
                Email = userExist.Email,
                Success = true,
                Errors = new List<string>()
            };
        }

        public async Task<UserResponseDto> RegisterUser(UserRegistrationRequestDto userToRegister)
        {
            var userExist = await userManager.FindByEmailAsync(userToRegister.Email);
            if(userExist != null)
            {
                return new UserResponseDto
                {
                    Success = false,
                    Errors = new List<string>() { "user already exist" }
                };
            }

            var newUser = new IdentityUser
            {
                Email = userToRegister.Email,
                UserName = userToRegister.Username
            };

            var isCreated = await userManager.CreateAsync(newUser, userToRegister.Password);
            if (isCreated.Succeeded)
            {
                return new UserResponseDto
                {
                    Username = userToRegister.Username,
                    Email = userToRegister.Email,
                    Success = true,
                    Errors = []
                };
            }
            else
            {
                return new UserResponseDto
                {
                    Success = false,
                    Errors = isCreated.Errors.Select(x => x.Description).ToList()
                };
            }
        }
    }

    public interface IUserService
    {
        public Task<List<IdentityUser>> GetUsers();
        public Task<UserResponseDto> LoginUser(UserLoginRequestDto userToLogin);
        public Task<UserResponseDto> RegisterUser(UserRegistrationRequestDto userToRegister);
        public Task<UserResponseDto> GetUserDetails(string email);
    }

    

}
