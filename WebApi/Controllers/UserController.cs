using Application.Constants;
using Application.DTOs.UsersDto;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Filters;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]/{id:guid}")]
    [ValidateModel]

    public class UserController(IUserService userService, IConfiguration configuration) : ControllerBase
    {

        [HttpGet()]
        public async Task<IActionResult> GetUserInformation(Guid id)
        {
            var result = await userService.GetInformationByID(id);
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
        [HttpPut()]
        [Authorize]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] CreateUpdateUserDto requestUser)
        {
            try
            {
                var result = await userService.UpdateUserAsync(id, requestUser);   
                if(!result.IsSuccess)
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
    }
}
