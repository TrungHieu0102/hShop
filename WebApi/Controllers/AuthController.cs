using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("Signup")]
        public async Task<IActionResult> SignUp([FromBody] SignUpDto signUpDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Invalid registration data", Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }

            var (isSuccess, error) = await _authService.SignUpAsync(signUpDto);
            if (isSuccess)
            {
                return Ok(new { Message = "User registered successfully" });
            }
            return BadRequest(new { Message = "Registration failed", Error = error });
        }

        [HttpPost("Signin")]
        public async Task<IActionResult> SignIn([FromBody] SignInDto signInDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { Message = "Invalid login data", Errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage) });
            }

            var result = await _authService.SignInAsycn(signInDto);
            if (!result.IsSuccess)
            {
                return Unauthorized(new { Message = "Login failed", Error = result.Message });
            }

            var response = new AuthResponseDto
            {
                AccessToken = result.AccessToken,
                Message = "Login successful",
                RefreshToken = result.RefreshToken,
                Expiration =result.Expiration,
                IsSuccess = true
            };

            return Ok(response);
        }

        // Đăng xuất
        [HttpPost("Signout")]
        public async Task<IActionResult> SignOutUser()
        {
            await _authService.SignOutAsync();
            return Ok(new { Message = "Logged out successfully" });
        }
    }
}
