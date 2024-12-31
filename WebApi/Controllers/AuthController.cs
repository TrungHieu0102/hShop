using Application.DTOs.AuthsDto;
using Application.Interfaces;
using Application.Model;
using Core.Model.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApi.Filters;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ValidateModel]

    public class AuthController(IAuthService authService)
        : ControllerBase
    {

        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUp([FromForm] SignUpDto signUpDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Invalid registration data", Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }

            var (isSuccess, error) = await authService.SignUpAsync(signUpDto);
            if (isSuccess)
            {
                return Ok(new { Message = "User registered successfully" });
            }
            return BadRequest(new { Message = "Registration failed", Error = error });
        }

        [HttpPost("sign-in")]
        public async Task<IActionResult> SignIn([FromBody] SignInDto signInDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Invalid login data", Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }
            try
            {

                var result = await authService.SignInAsycn(signInDto);
                if (!result.IsSuccess)
                {
                    return Unauthorized(new { Message = "Login failed", Error = result.Message });
                }

                var response = new AuthResponseDto
                {
                    AccessToken = result.AccessToken,
                    Message = "Login successful",
                    RefreshToken = result.RefreshToken,
                    Expiration = result.Expiration,
                    IsSuccess = true
                };

                return Ok(response);

            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }

        }
        [HttpPost("sign-out")]
        [Authorize]
        public async Task<IActionResult> SignOutUser()
        {
            await authService.SignOutAsync();
            return Ok(new { Message = "Logged out successfully" });
        }
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery]string userId, [FromQuery]string token)
        {
            var result = await authService.ConfirmEmail(userId, token);
            return Content(result);
        }
        [HttpPost("request-password-change")]
        public async Task<IActionResult> RequestPasswordChange([FromBody] RequestPasswordChange request)
        {
            var result = await authService.RequestPasswordChangeAsync(request.Email, User);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.First().Description);
            }

            return Ok("OTP sent to your email.");
        }
        [HttpPost("confirm-password-change")]
        public async Task<IActionResult> ConfirmPasswordChange([FromBody] ConfirmPasswordChangeRequest model)
        {
            var result = await authService.ConfirmPasswordChangeAsync(model);

            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.First().Description);
            }

            return Ok("Password changed successfully.");
        }
        [AllowAnonymous]
        [HttpPost("login-google")]
        public async Task<IActionResult> GoogleAuthenticate([FromBody] GoogleUserRequest request)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState.Values.SelectMany(it => it.Errors).Select(it => it.ErrorMessage));
            return Ok(await authService.AuthenticateGoogleUserAsync(request));
        }
    }
}
