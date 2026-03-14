using FiguraSp.Users.Model.DTOs.Requests;
using FiguraSp.Users.Model.DTOs.Responses;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace FiguraSp.Users.Service.Services
{
    public class UserService(UserManager<IdentityUser> userManager) : IUserService
    {
        public async Task<UserRegisterResponseDto> GetUserDetails(string email)
        {
            var userExist = await userManager.FindByEmailAsync(email);
            if (userExist == null)
            {
                return new UserRegisterResponseDto
                {
                    Success = false,
                    Errors = new List<string>() { "user does not exist" }
                };
            }

            return new UserRegisterResponseDto
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

        public async Task<UserRegisterResponseDto> RegisterUser(UserRegistrationRequestDto userToRegister)
        {
            var userExist = await userManager.FindByEmailAsync(userToRegister.Email);
            if(userExist != null)
            {
                return new UserRegisterResponseDto
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
                return new UserRegisterResponseDto
                {
                    Username = userToRegister.Username,
                    Email = userToRegister.Email,
                    Success = true,
                    Errors = []
                };
            }
            else
            {
                return new UserRegisterResponseDto
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

        public Task<UserRegisterResponseDto> RegisterUser(UserRegistrationRequestDto userToRegister);

        public Task<UserRegisterResponseDto> GetUserDetails(string email);
    }

    

}
