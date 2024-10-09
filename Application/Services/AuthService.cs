using Application.Constants;
using Application.DTOs;
using Application.Extentions;
using Application.Interfaces;
using Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;
        public AuthService(UserManager<User> userManager, IConfiguration configuration, SignInManager<User> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _roleManager = roleManager;
        }
        public async Task<AuthResponseDto> SignInAsycn(SignInDto signInDto)
        {
            var user = await _userManager.FindByEmailAsync(signInDto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, signInDto.Password))
            {
                return new AuthResponseDto { IsSuccess = false, Message = "Invalid Email or Password" };
            }
            var result = await _signInManager.PasswordSignInAsync(signInDto.Email, signInDto.Password, false, false);

            if (!result.Succeeded)
            {
                return new AuthResponseDto { IsSuccess = false, Message = "Invalid Email or Password" };
            }
            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, signInDto.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var userRole = await _userManager.GetRolesAsync(user);
            foreach (var role in userRole)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role.ToString()));                           
            }
            var authKey = Encoding.UTF8.GetBytes(_configuration["JWT:Key"]);
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: authClaims,
                expires: DateTime.Now.AddMinutes(1),
                signingCredentials: new SigningCredentials(new SymmetricSecurityKey(authKey), SecurityAlgorithms.HmacSha256)
            );

            var refreshToken = RefreshToken.GenerateRefreshToken(); 
            await SaveRefreshToken(user.Id, refreshToken); 

            return new AuthResponseDto
            {
                AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                RefreshToken = refreshToken,
                IsSuccess = true,
                Message = string.Empty,
                Expiration = user.RefreshTokenExpiryTime.Value
            };
        }
        public async Task<(bool, string)> SignUpAsync(SignUpDto signUpDto)
        {
            var user = new User
            {
                FullName = signUpDto.FullName,
                Email = signUpDto.Email,
                UserName = signUpDto.Email
            };

            var result = await _userManager.CreateAsync(user, signUpDto.Password);
            if (!result.Succeeded)
            {
                return (false, string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            if (!await _roleManager.RoleExistsAsync(RoleConstants.User))
            {
                await _roleManager.CreateAsync(new IdentityRole(RoleConstants.User));
            }
            await _userManager.AddToRoleAsync(user, RoleConstants.User);

            return (true, string.Empty);
        }
        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }
        // Phương thức lưu refresh token vào DB
        private async Task SaveRefreshToken(string userId, string refreshToken)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                user.RefreshToken = refreshToken; 
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7); 

                await _userManager.UpdateAsync(user); 
            }
        }
    }

}

