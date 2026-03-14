using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FiguraSp.Users.Api.Controllers
{
    [Route("api/[controller]")] // http://localhost:5000/api/user
    [ApiController]
    public class UserController(UserManager<IdentityUser> userManager) : ControllerBase
    {
        [HttpGet]
        [Route("Users")]
        public async Task<ActionResult> GetUsers()
        {
            var users = await userManager.Users.ToListAsync();

            return Ok(users);
        }
    }
}
