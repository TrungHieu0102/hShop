using Application.Interfaces;
using Core.Entities;
using Core.Model.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
#nullable disable
namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly UserManager<User> _userManager;
        public TokenController(ITokenService tokenService, UserManager<User> userManager)
        {
            _tokenService = tokenService;
            _userManager = userManager;
        }
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
            var principal = _tokenService.GetPrincipalFromExpiredToken(accessToken);
            if(principal == null || principal.Identity == null || principal.Identity.Name == null)
            {
                return BadRequest("Invalid client request");
            }
            var username = principal.Identity.Name;
            var user = await _userManager.FindByNameAsync(username);
            if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            {
                return BadRequest("Invalid client request");
            }
            var newAccessToken = _tokenService.GenerateAccessToken(principal.Claims);
            var newRefreshToken = _tokenService.GenerateRefreshToken();
            user.RefreshToken = newRefreshToken;
            await _userManager.UpdateAsync(user);
            return new AuthenticatedResult
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken
            };

        }
        [HttpPost][Authorize]
        [Route("revoke")]
        public async Task<IActionResult> Revoke()
        {
           
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                return BadRequest();
            }
            user.RefreshToken = null;
            await _userManager.UpdateAsync(user);
            return NoContent();
        }


    }
}
