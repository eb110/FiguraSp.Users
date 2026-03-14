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
    }
}
