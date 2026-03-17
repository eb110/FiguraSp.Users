using FiguraSp.Users.Model.Data;
using FiguraSp.Users.Model.DTOs.Requests;
using FiguraSp.Users.Model.DTOs.Responses;
using FiguraSp.Users.Model.Entity;
using FiguraSp.Users.Service.Configuration;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FiguraSp.Users.Service.Services
{
    public class UserService(UserManager<IdentityUser> userManager, UsersDbContext context, TokenValidationParameters tokenValidationParameters) : IUserService
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

            var userResponseDto = await GenerateJwtToken(userExist, jwtConfiguration);

            return userResponseDto;
        }

        public async Task<UserResponseDto> RegisterUser(UserRegistrationRequestDto userToRegister, JwtConfiguration jwtConfiguration)
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
                await userManager.AddToRoleAsync(newUser, "User");
                var userResponseDto = await GenerateJwtToken(newUser, jwtConfiguration);
                return userResponseDto;
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

        private async Task<UserResponseDto> GenerateJwtToken(IdentityUser user, JwtConfiguration jwtConfiguration)
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

            //refresh token
            var refreshToken = new RefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                //refresh token value
                Token = RandomString(35) + Guid.NewGuid(),
                ExpiresOnUtc = DateTime.UtcNow.AddDays(1)
            };

            context.RefreshTokens.Add(refreshToken);
            await context.SaveChangesAsync();

            return new UserResponseDto
            {
                Username = user.UserName,
                Email = user.Email,
                Success = true,
                AccessToken = accessToken,
                RefreshToken = refreshToken.Token,          
            };
        }

        private static string RandomString(int length)
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(x => x[random.Next(x.Length)]).ToArray());
        }

        public async Task<UserResponseDto> RefreshToken(TokenRequestDto tokenRequest, JwtConfiguration jwtConfiguration)
        {
            RefreshToken? refreshToken = await context.RefreshTokens.Include(r => r.User).FirstOrDefaultAsync(r => r.Token == tokenRequest.RefreshToken);
            if(refreshToken == null || refreshToken.ExpiresOnUtc < DateTime.UtcNow)
            {
                return new UserResponseDto
                {
                    Errors = new List<string>() { "expired refresh token" }
                };             
            }

            var dbUser = await userManager.FindByIdAsync(refreshToken.UserId);
            var tokensResponse = await GenerateJwtToken(dbUser!, jwtConfiguration);
            return tokensResponse;
        }
    }

    public interface IUserService
    {
        public Task<List<IdentityUser>> GetUsers();
        public Task<UserResponseDto> LoginUser(UserLoginRequestDto userToLogin, JwtConfiguration jwtConfiguration);
        public Task<UserResponseDto> RefreshToken(TokenRequestDto tokenRequest, JwtConfiguration jwtConfiguration);
        public Task<UserResponseDto> RegisterUser(UserRegistrationRequestDto userToRegister, JwtConfiguration jwtConfiguration);
        public Task<UserResponseDto> GetUserDetails(string email);
    }

    

}
