using FiguraSp.Users.Api.Configuration;
using FiguraSp.Users.Model.DTOs.Requests;
using FiguraSp.Users.Model.DTOs.Responses;
using FiguraSp.Users.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace FiguraSp.Users.Api.Controllers
{
    [Route("api/[controller]")] // http://localhost:5000/api/user
    [ApiController]
    public class UserController(IUserService userService, IOptionsMonitor<JwtConfiguration> optionsMonitor) : ControllerBase
    {
        private readonly JwtConfiguration jwtConfiguration = optionsMonitor.CurrentValue;

        [HttpGet]
        [Route("Users")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<IdentityUser>>> GetUsers()
        {
            var users = await userService.GetUsers();

            return Ok(users);
        }

        [HttpPost]
        [Route("Login")]
        public async Task<ActionResult<UserResponseDto>> Login([FromBody] UserLoginRequestDto user)
        {
            if (ModelState.IsValid)
            {
                var userLoginResponse = await userService.LoginUser(user, jwtConfiguration);
                if(userLoginResponse.Success)
                {
                    return CreatedAtAction("GetUser", new { user.Email }, userLoginResponse);
                }
                return BadRequest(userLoginResponse);
            }
            return BadRequest(new UserResponseDto
            {
                Success = false,
                Errors = new List<string>() { "Reqest with inavlid credentials" }
            });
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<UserResponseDto>> GetUser(string email)
        {
            var result = await userService.GetUserDetails(email);
            if(result.Success)
            {
                return Ok(result);
            }
            return BadRequest(result);
        }

        [HttpPost]
        [Route("Register")]
        public async Task<ActionResult<UserResponseDto>> Register([FromBody] UserRegistrationRequestDto userToRegister)
        {
            if(ModelState.IsValid)
            {
                var result = await userService.RegisterUser(userToRegister);
                if(result.Success)
                {
                    return CreatedAtAction("GetUser", new { result.Email }, result);
                }
                return BadRequest(result);
            }

            return BadRequest(new UserResponseDto
            {
                Success = false,
                Errors = new List<string>() { "Reqest with inavlid credentials" }
            });
        }
    }
}
