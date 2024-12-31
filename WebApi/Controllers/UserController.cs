using Application.Constants;
using Application.DTOs.UsersDto;
using Application.Interfaces;
using Application.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Filters;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ValidateModel]
    public class UserController(IUserService userService) : ControllerBase
    {
        [HttpGet("{userId:guid}")]

        public async Task<IActionResult> GetUserInformation([FromRoute] Guid userId)
        {
            var result = await userService.GetInformationById(userId);
            if (!result.IsSuccess)
            {
                return NotFound(result.Message);
            }

            return Ok(result.Data);
        }

        [HttpDelete("{userId:guid}")]
        [Authorize(Roles = RoleConstants.Admin)]
        public async Task<IActionResult> DeleteUser([FromRoute] Guid userId)
        {
            var result = await userService.DeleteUserById(userId);
            if (!result)
            {
                return NotFound("User not found");
            }

            return Ok("User deleted successfully");
        }

        [HttpPut("{userId:guid}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser([FromRoute] Guid userId, [FromBody] CreateUpdateUserDto requestUser)
        {
            try
            {
                var result = await userService.UpdateUserAsync(userId, requestUser);
                if (!result.IsSuccess)
                {
                    return BadRequest(result.Message);
                }

                return Ok(result.Data);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }
        
        [HttpPut("assign-roles/{userId:guid}")]
        [Authorize(Roles = RoleConstants.Admin)]
        public async Task<IActionResult> AssignRolesToUser([FromRoute]Guid userId, [FromBody] string[] roles)
        {
            var result = await userService.UpdateUserRoleAsync(userId, roles);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Message);
        }

        [HttpPut("unassign-roles/{userId:guid}")]
        [Authorize(Roles = RoleConstants.Admin)]
        public async Task<IActionResult> UnassignRolesToUser([FromRoute]Guid userId, [FromBody] string[] roles)
        {
            var result = await userService.DeleteUserRolesAsync(userId, roles);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUsers(string? keyword, int pageIndex = 1, int pageSize = 10)
        {
            var result = await userService.GetAllUsersPaging(keyword, pageIndex, pageSize);
            if (!result.IsSuccess)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        [HttpPost("set-password/{userId:guid}")]
        public async Task<IActionResult> SetPassword([FromRoute]Guid userId, [FromBody] SetPasswordRequest passwordRequest)
        {
            var result = await userService.SetPassword(userId, passwordRequest);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Message);
        }

       
    }
}