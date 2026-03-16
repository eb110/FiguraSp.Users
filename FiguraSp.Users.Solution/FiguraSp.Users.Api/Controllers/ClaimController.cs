using FiguraSp.SharedLibrary.Responses;
using FiguraSp.Users.Model.DTOs.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FiguraSp.Users.Api.Controllers
{
    [Route("api/[controller]")] // http://localhost:5000/api/claim
    [ApiController]
    [Authorize(Roles = "Admin", Policy = "Administrator")]
    public class ClaimController(UserManager<IdentityUser> userManager): ControllerBase
    {
        [HttpGet]
        [Route("Claims")]
        public async Task<ActionResult<IList<Claim>>> GetAllClaims(string email)
        {
            //userExist
            var userExist = await userManager.FindByEmailAsync(email);
            if (userExist == null)
            {
                return BadRequest(new DefaultResponse
                {
                    Errors = new List<string>() { $"The user {email} does not exist" }
                });
            }

            var userClaims = await userManager.GetClaimsAsync(userExist);
            return Ok(userClaims);
        }

        [HttpPost]
        [Route("AddClaimsToUser")]
        public async Task<ActionResult> AddClaimsToUser(string email, string claimKey, string claimValue)
        {
            var userExist = await userManager.FindByEmailAsync(email);
            if (userExist == null)
            {
                return BadRequest(new DefaultResponse
                {
                    Errors = new List<string>() { $"The user {email} does not exist" }
                });
            }

            var userClaims = await userManager.GetClaimsAsync(userExist);
            foreach(var claim in userClaims)
            {      
                if(claim.Type.Equals(claimKey) && claim.Value.Equals(claimValue))
                {
                    return BadRequest(new DefaultResponse { Errors = new List<string>() { "Duplicated claim"} });
                }
            }

            var userClaim = new Claim(claimKey, claimValue);

            var result = await userManager.AddClaimAsync(userExist, userClaim);

            if (result.Succeeded)
            {
                return StatusCode(201, new ClaimResponseDto
                {
                    Success = true,
                    ClaimKey = claimKey,
                    ClaimValue = claimValue
                });
            }
            return BadRequest(new DefaultResponse
            {
                Errors = new List<string>() { $"claim has not been added" }
            });
        }
    }
}
