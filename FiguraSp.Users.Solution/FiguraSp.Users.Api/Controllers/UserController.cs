using FiguraSp.Users.Model.DTOs.Requests;
using FiguraSp.Users.Model.DTOs.Responses;
using FiguraSp.Users.Service.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FiguraSp.Users.Api.Controllers
{
    [Route("api/[controller]")] // http://localhost:5000/api/user
    [ApiController]
    public class UserController(IUserService userService) : ControllerBase
    {
        [HttpGet]
        [Route("Users")]
        public async Task<ActionResult<List<IdentityUser>>> GetUsers()
        {
            var users = await userService.GetUsers();

            return Ok(users);
        }

        [HttpGet]
        public async Task<ActionResult<UserRegisterResponseDto>> GetUser(string email)
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
        public async Task<ActionResult<UserRegisterResponseDto>> Register([FromBody] UserRegistrationRequestDto userToRegister)
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

            return BadRequest(new UserRegisterResponseDto
            {
                Success = false,
                Errors = new List<string>() { "Reqest with inavlid credentials" }
            });
        }
    }
}
