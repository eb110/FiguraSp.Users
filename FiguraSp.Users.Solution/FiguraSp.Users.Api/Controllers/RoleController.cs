using FiguraSp.Users.Model.DTOs.Responses;
using FiguraSp.Users.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FiguraSp.Users.Api.Controllers
{
    [Route("api/[controller]")] // http://localhost:5000/api/role
    [ApiController]
    [Authorize(Roles = "Admin", Policy = "Administrator")]
    public class RoleController(IRoleService roleService) : ControllerBase
    {
        [HttpGet]
        [Route("Roles")]
        public async Task<ActionResult<List<IdentityRole>>> GetAllRoles()
        {
            var roles = await roleService.GetRoles();
            return Ok(roles);
        }

        [HttpPost]
        public async Task<ActionResult<RoleResponseDto>> CreateRole(string roleName)
        {
            var response = await roleService.AddRole(roleName);
            if(response.Success)
            {
                return CreatedAtAction("GetRoleByName", new { roleName }, response);
            }

            return BadRequest(response);
        }

        [HttpGet]
        public async Task<ActionResult<RoleResponseDto>> GetRoleByName(string roleName)
        {
            var response = await roleService.GetRole(roleName);
            if(response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);       
        }

        [HttpDelete]
        public async Task<ActionResult> DeleteRole(string roleName)
        {
            var response = await roleService.DeleteRole(roleName);
            if(response.Success)
            {
                return NoContent();
            }
            return BadRequest(response);   
        }

        [HttpPost]
        [Route("AddUserToRole")]
        public async Task<ActionResult<RoleResponseDto>> AddUserToRole(string email, string roleName)
        {
            var response = await roleService.AddUserToRole(email, roleName);
            if (response.Success)
            {
                return StatusCode(201, response);
            }
            return BadRequest(response);
        }

        [HttpGet]
        [Route("GetUserRoles")]
        public async Task<ActionResult<IList<string>>> GetUserRoles(string email)
        {
            var response = await roleService.GetUserRoles(email);
            if (response != null)
            {
                return Ok(response);
            }
            return BadRequest("wrong credentials");
        }

        [HttpPost]
        [Route("RemoveUserFromRole")]
        public async Task<ActionResult<RoleResponseDto>> RemoveUserFromRoles(string email, string roleName)
        {
            var response = await roleService.RemoveUserFromRole(email, roleName);
            if (response.Success)
            {
                return NoContent();
            }
            return BadRequest(response);
        }

        [HttpGet]
        [Route("AttachUserRoleToUsers")]
        public async Task<ActionResult> AddUserRoleToUsers()
        {
            await roleService.AddUserRoleToUsers();
            return Ok();
        }
    }
}
