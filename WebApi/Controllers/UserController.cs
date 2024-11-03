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
    public class UserController(IUserService userService, IConfiguration configuration) : ControllerBase
    {
        [HttpGet()]
        public async Task<IActionResult> GetUserInformation(Guid id)
        {
            var result = await userService.GetInformationById(id);
            if (!result.IsSuccess)
            {
                return NotFound(result.Message);
            }

            return Ok(result.Data);
        }

        [HttpDelete()]
        [Authorize(Roles = RoleConstants.Admin)]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var result = await userService.DeleteUserById(id);
            if (!result)
            {
                return NotFound("User not found");
            }

            return Ok("User deleted successfully");
        }

        [HttpPut]
        [Authorize]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] CreateUpdateUserDto requestUser)
        {
            try
            {
                var result = await userService.UpdateUserAsync(id, requestUser);
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

        [HttpPut("api/User/assign-roles")]
        [Authorize(Roles = RoleConstants.Admin)]
        public async Task<IActionResult> AssignRolesToUser(Guid id, [FromBody] string[] roles)
        {
            var result = await userService.UpdateUserRoleAsync(id, roles);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Message);
        }

        [HttpPut("api/User/unassign-roles")]
        [Authorize(Roles = RoleConstants.Admin)]
        public async Task<IActionResult> UnassignRolesToUser(Guid id, [FromBody] string[] roles)
        {
            var result = await userService.DeleteUserRolesAsync(id, roles);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Ok();
        }

        [HttpGet("get-all-users")]
        public async Task<IActionResult> GetAllUsers(string? keyword, int pageIndex = 1, int pageSize = 10)
        {
            var result = await userService.GetAllUsersPaging(keyword, pageIndex, pageSize);
            if (!result.IsSuccess)
            {
                return BadRequest();
            }

            return Ok(result);
        }

        [HttpPost("set-password")]
        public async Task<IActionResult> SetPassword(Guid id, [FromBody] SetPasswordRequest passwordRequest)
        {
            var result = await userService.SetPassword(id, passwordRequest);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Message);
        }

        [HttpPut("{id}")]
        [ValidateModel]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] UpdateUserRequest requestUser)
        {
            var result = await userService.UpdateUser(id, requestUser);
            if (!result.IsSuccess)
            {
                return BadRequest(result.Message);
            }

            return Ok(result.Data);
        }
    }
}