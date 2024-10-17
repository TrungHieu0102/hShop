using Application.Constants;
using Application.DTOs.AuthsDto;
using Application.DTOs.RolesDto;
using Application.Extentions;
using Application.Interfaces;
using Application.Model;
using Core.Entities;
using Core.SeedWorks;
using Core.SeedWorks.Constants;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
using System.Text;
using System.Text.Json;


namespace Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        public AuthService(UserManager<User> userManager, IConfiguration configuration, SignInManager<User> signInManager, RoleManager<Role> roleManager, IEmailService emailService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _roleManager = roleManager;
            _emailService = emailService;
        }
        public async Task<AuthResponseDto> SignInAsycn(SignInDto signInDto)
        {
            var user = await _userManager.FindByEmailAsync(signInDto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, signInDto.Password))
            {
                return new AuthResponseDto { IsSuccess = false, Message = "Invalid Email or Password" };
            }
            var result = await _signInManager.PasswordSignInAsync(signInDto.Email, signInDto.Password, false, false);
            var emailConfirmed = await _userManager.IsEmailConfirmedAsync(user);
            if (!result.Succeeded)
            {
                return new AuthResponseDto { IsSuccess = false, Message = "Invalid Email or Password" };
            }
            if (!emailConfirmed)
            {
                await _emailService.SendConfirmationEmail(signInDto.Email, user);
                return new AuthResponseDto { IsSuccess = false, Message = "Please confirm your email " };
            }
            var roles = await _userManager.GetRolesAsync(user);

            var permission = await GetPermissionAsync(user.Id.ToString());
            var authClaims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(UserClaims.Id, user.Id.ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.Email),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(UserClaims.FirstName, user.FirstName),
                    new Claim(UserClaims.Roles, string.Join(";", roles)),
                    new Claim(UserClaims.Permissions, JsonSerializer.Serialize(permission)),
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
            await _emailService.SendConfirmationEmail(signUpDto.Email, user);
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
        public async Task<string> ConfirmEmail(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (userId == null || token == null)
            {
                return "Link expired";
            }
            else if (user == null)
            {
                return "User not Found";
            }
            else
            {
                token = token.Replace(" ", "+");
                var result = await _userManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                {
                    return "Thank you for confirming your email";
                }
                else
                {
                    return "Email not confirmed";
                }
            }
        }
        private async Task<List<string>> GetPermissionAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            var roles = await _userManager.GetRolesAsync(user);
            var permissions = new List<string>();
            var allPermissions = new List<RoleClaimsDto>();
            if (roles.Contains(Roles.Admin))
            {
                var types = typeof(Permissions).GetTypeInfo().DeclaredNestedTypes;
                foreach (var type in types)
                {
                    allPermissions.GetPermissions(type);
                }
                permissions.AddRange(allPermissions.Select(x => x.Value));
            }
            else
            {
                foreach (var roleName in roles)
                {
                    var role = await _roleManager.FindByNameAsync(roleName);
                    var claims = await _roleManager.GetClaimsAsync(role);
                    var roleClaimValues = claims.Select(x => x.Value).ToList();
                    permissions.AddRange(roleClaimValues);
                }
            }
            return permissions.Distinct().ToList();
        }

        public async Task<IdentityResult> RequestPasswordChangeAsync(string mail, ClaimsPrincipal user)
        {
            User currentUser;

            if (user != null && user.Identity != null && user.Identity.IsAuthenticated)
            {
                var email = user.FindFirst(ClaimTypes.Email)?.Value;
                if (email == null)
                {
                    return IdentityResult.Failed(new IdentityError { Description = "Email claim not found." });
                }
                currentUser = await _userManager.FindByEmailAsync(email);
            }
            else
            {
                currentUser = await _userManager.FindByEmailAsync(mail);
                if (currentUser == null)
                {
                    return IdentityResult.Failed(new IdentityError { Description = "User not found." });
                }
            }
            var otp = PasswordExtensions.GenerateOtp();
            currentUser.OTP = otp;
            await _userManager.UpdateAsync(currentUser);
            await _emailService.SendEmailAsync(currentUser.Email, "Mã xác nhận đổi mật khẩu", $"Mã OTP của bạn là: {otp}", false);

            return IdentityResult.Success;
        }
        public async Task<IdentityResult> ConfirmPasswordChangeAsync(ConfirmPasswordChangeRequest request)
        {
            var user = await _userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });
            }
            if (user.OTP != request.OtpCode)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Invalid OTP code." });
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, request.NewPassword);

            if (result.Succeeded)
            {
                user.OTP = null;
                await _userManager.UpdateAsync(user);
            }

            return result;
        }

        //==========================//
      
    }
}

