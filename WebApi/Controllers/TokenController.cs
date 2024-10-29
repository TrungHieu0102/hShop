using Application.Interfaces;
using Core.Entities;
using Core.Model.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApi.Filters;
#nullable disable
namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [ValidateModel]

    public class TokenController(ITokenService tokenService, UserManager<User> userManager) : ControllerBase
    {
        [HttpPost]
        [Route("refresh")]
        public async Task<ActionResult<AuthenticatedResult>> Refresh(TokenRequest tokenRequest)
        {
            if (tokenRequest is null)
            {
                return BadRequest("Invalid client request");
            }
            var accessToken = tokenRequest.AccessToken;
            var refreshToken = tokenRequest.RefreshToken;
            var principal = tokenService.GetPrincipalFromExpiredToken(accessToken);
            if(principal.Identity?.Name == null)
            {
                return BadRequest("Invalid client request");
            }
            var username = principal.Identity.Name;
            var user = await userManager.FindByNameAsync(username);
            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                return BadRequest("Invalid client request");
            }
            var newAccessToken = tokenService.GenerateAccessToken(principal.Claims);
            // var newRefreshToken = tokenService.GenerateRefreshToken();
            // user.RefreshToken = newRefreshToken;
            await userManager.UpdateAsync(user);
            return new AuthenticatedResult
            {
                AccessToken = newAccessToken,
                // RefreshToken = newRefreshToken
            };

        }
        [HttpPost][Authorize]
        [Route("revoke")]
        public async Task<IActionResult> Revoke()
        {
           
            var user = await userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return BadRequest();
            }
            user.RefreshToken = null;
            await userManager.UpdateAsync(user);
            return NoContent();
        }


    }
}
