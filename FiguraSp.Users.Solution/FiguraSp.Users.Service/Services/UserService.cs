using FiguraSp.Users.Api.Configuration;
using FiguraSp.Users.Model.DTOs.Requests;
using FiguraSp.Users.Model.DTOs.Responses;
using FiguraSp.Users.Service.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

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

        public async Task<UserResponseDto> LoginUser(UserLoginRequestDto userToLogin, JwtConfiguration jwtConfiguration)
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

            var jwtTokenResponse = await GenerateJwtToken(userExist, jwtConfiguration);

            return new UserResponseDto
            {
                Username = userExist.UserName,
                Email = userExist.Email,
                AccessToken = jwtTokenResponse.Token,
                Success = true,
                Errors = new List<string>()
            };
        }

        public async Task<UserResponseDto> RegisterUser(UserRegistrationRequestDto userToRegister)
        {
            var userExist = await userManager.FindByEmailAsync(userToRegister.Email);
            if (userExist != null)
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

        private async Task<AuthenticationResult> GenerateJwtToken(IdentityUser user, JwtConfiguration jwtConfiguration)
        {
            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfiguration.SecretKey));
            var credentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256Signature);
            var claims = await GetAllValidClaims(user);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddSeconds(jwtConfiguration.ExpirationInSeconds),
                SigningCredentials = credentials,
                Issuer = jwtConfiguration.Issuer,
                Audience = jwtConfiguration.Audience,
            };

            var tokenHandler = new JsonWebTokenHandler();
            string accessToken = tokenHandler.CreateToken(tokenDescriptor);
            return new AuthenticationResult
            {
                Success = true,
                Token = accessToken,
            };
        }

        private async Task<List<Claim>> GetAllValidClaims(IdentityUser user)
        {
            var roles = await userManager.GetRolesAsync(user);
            var userClaims = await userManager.GetClaimsAsync(user);

            List<Claim> claims =
            [
                  new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Email, user.Email!),
                  new Claim(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub, user.Id),
                  ..roles.Select(r => new Claim(ClaimTypes.Role, r)),
                  ..userClaims
            ];

            return claims;
        }
    }

    public interface IUserService
    {
        public Task<List<IdentityUser>> GetUsers();
        public Task<UserResponseDto> LoginUser(UserLoginRequestDto userToLogin, JwtConfiguration jwtConfiguration);
        public Task<UserResponseDto> RegisterUser(UserRegistrationRequestDto userToRegister);
        public Task<UserResponseDto> GetUserDetails(string email);
    }

    

}
