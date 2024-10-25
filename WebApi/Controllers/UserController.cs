using Application.Constants;
using Application.DTOs.UsersDto;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Filters;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ValidateModel]

    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IConfiguration _configuration;
        public UserController(IUserService userService, IConfiguration configuration)
        {
            _userService = userService;
            _configuration = configuration;
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserInformation(Guid id)
        {
            var result = await _userService.GetInformationByID(id);
            if (!result.IsSuccess)
            {
                return NotFound(result.Message);
            }
            return Ok(result.Data);
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = RoleConstants.Admin)]
        public async Task<IActionResult> DeleteUser(Guid id)
        {
            var result = await _userService.DeleteUserById(id);
            if (!result)
            {
                return NotFound("User not found");
            }
            return Ok("User deleted successfully");
        }
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUser(Guid id, [FromBody] CreateUpdateUserDto requestUser)
        {
            try
            {
                var result = await _userService.UpdateUserAsync(id, requestUser);   
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
