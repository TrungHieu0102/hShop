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
        private readonly RoleManager<Role> _roleManager;
        private readonly IConfiguration _configuration;
        public AuthService(UserManager<User> userManager, IConfiguration configuration, SignInManager<User> signInManager, RoleManager<Role> roleManager)
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
                new Claim(ClaimTypes.Name, signInDto.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var userRole = await _userManager.GetRolesAsync(user);
            foreach (var role in userRole)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role.ToString()));
            }
            var jwtKey = _configuration["JWT:Key"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new InvalidOperationException("JWT Key is not configured.");
            }
            var authKey = Encoding.UTF8.GetBytes(jwtKey);
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: authClaims,
                expires: DateTime.Now.AddMinutes(30),
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
                Expiration = user.RefreshTokenExpiryTime ?? DateTime.MinValue
            };
        }
        public async Task<(bool, string)> SignUpAsync(SignUpDto signUpDto)
        {
            var user = new User
            {
                LastName = signUpDto.LastName,
                FirstName = signUpDto.FirstName,
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
                await _roleManager.CreateAsync(new Role { Name = RoleConstants.User, DisplayName = RoleConstants.User });
            }
            await _userManager.AddToRoleAsync(user, RoleConstants.User);

            return (true, string.Empty);
        }
        public async Task SignOutAsync()
        {
            await _signInManager.SignOutAsync();
        }
        private async Task SaveRefreshToken(Guid userId, string refreshToken)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user != null)
            {
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

                await _userManager.UpdateAsync(user);
            }
        }
    }

}

