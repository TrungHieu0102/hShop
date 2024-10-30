using Application.Constants;
using Application.DTOs.AuthsDto;
using Application.DTOs.RolesDto;
using Application.Extensions;
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
using Application.Common.TemplateEmail;


namespace Application.Services
{
    public class AuthService(
        UserManager<User> userManager,
        IConfiguration configuration,
        SignInManager<User> signInManager,
        RoleManager<Role> roleManager,
        IEmailService emailService,
        ICacheService cacheService)
        : IAuthService
    {
        private readonly ICacheService _cacheService = cacheService;

        public async Task<AuthResponseDto> SignInAsycn(SignInDto signInDto)
        {
            var user = await userManager.FindByEmailAsync(signInDto.Email);
            if (user == null || !await userManager.CheckPasswordAsync(user, signInDto.Password))
            {
                return new AuthResponseDto { IsSuccess = false, Message = "Invalid Email or Password" };
            }
            var result = await signInManager.PasswordSignInAsync(signInDto.Email, signInDto.Password, false, false);
            var emailConfirmed = await userManager.IsEmailConfirmedAsync(user);
            if (!result.Succeeded)
            {
                return new AuthResponseDto { IsSuccess = false, Message = "Invalid Email or Password" };
            }
            if (!emailConfirmed)
            {
                await emailService.SendConfirmationEmail(signInDto.Email, user);
                return new AuthResponseDto { IsSuccess = false, Message = "Please confirm your email " };
            }
            var roles = await userManager.GetRolesAsync(user);

            var permission = await GetPermissionAsync(user.Id.ToString());
            var authClaims = new List<Claim>
            {
                new(JwtRegisteredClaimNames.Email, user.Email),
                    new Claim(UserClaims.Id, user.Id.ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.Email),
                    new Claim(ClaimTypes.Name, user.UserName),
                    new Claim(UserClaims.FirstName, user.FirstName),
                    new Claim(UserClaims.Roles, string.Join(";", roles)),
                    new Claim(UserClaims.Permissions, JsonSerializer.Serialize(permission)),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };
            var userRole = await userManager.GetRolesAsync(user);

            foreach (var role in userRole)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, role.ToString()));
            }
            var jwtKey = configuration["JWT:Key"];
            if (string.IsNullOrEmpty(jwtKey))
            {
                throw new InvalidOperationException("JWT Key is not configured.");
            }
            var authKey = Encoding.UTF8.GetBytes(jwtKey);
            var token = new JwtSecurityToken(
                issuer: configuration["JWT:Issuer"],
                audience: configuration["JWT:Audience"],
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
            var result = await userManager.CreateAsync(user, signUpDto.Password);

            if (!result.Succeeded)
            {
                return (false, string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            if (!await roleManager.RoleExistsAsync(RoleConstants.User))
            {
                await roleManager.CreateAsync(new Role { Name = RoleConstants.User, DisplayName = RoleConstants.User });
            }
            await emailService.SendConfirmationEmail(signUpDto.Email, user);
            return (true, string.Empty);
        }
        public async Task SignOutAsync()
        {
            await signInManager.SignOutAsync();
        }
        private async Task SaveRefreshToken(Guid userId, string refreshToken)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user != null)
            {
                user.RefreshToken = refreshToken;
                user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

                await userManager.UpdateAsync(user);
            }
        }
        public async Task<string> ConfirmEmail(string userId, string token)
        {
            var user = await userManager.FindByIdAsync(userId);
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
                var result = await userManager.ConfirmEmailAsync(user, token);
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
            var user = await userManager.FindByIdAsync(userId);
            var roles = await userManager.GetRolesAsync(user);
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
                    var role = await roleManager.FindByNameAsync(roleName);
                    var claims = await roleManager.GetClaimsAsync(role);
                    var roleClaimValues = claims.Select(x => x.Value).ToList();
                    permissions.AddRange(roleClaimValues);
                }
            }
            return permissions.Distinct().ToList();
        }

        public async Task<IdentityResult> RequestPasswordChangeAsync(string mail, ClaimsPrincipal user)
        {
            User currentUser;

            if (user is { Identity.IsAuthenticated: true })
            {
                var email = user.FindFirst(ClaimTypes.Email)?.Value;
                if (email == null)
                {
                    return IdentityResult.Failed(new IdentityError { Description = "Email claim not found." });
                }
                currentUser = await userManager.FindByEmailAsync(email);
            }
            else
            {
                currentUser = await userManager.FindByEmailAsync(mail);
                if (currentUser == null)
                {
                    return IdentityResult.Failed(new IdentityError { Description = "User not found." });
                }
            }
            var otp = PasswordExtensions.GenerateOtp();
            var cacheKey = $"otp-{currentUser.Id}";
            await cacheService.SetCachedDataAsyncWithTime(cacheKey, otp, TimeSpan.FromMinutes(5));
            string body = GenerateEmailBody.GetEmailOTPBody(mail, otp);

            await emailService.SendEmailAsync(currentUser.Email, "Mã xác nhận đổi mật khẩu", body, true);

            return IdentityResult.Success;
        }
        public async Task<IdentityResult> ConfirmPasswordChangeAsync(ConfirmPasswordChangeRequest request)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user == null)
            {
                return IdentityResult.Failed(new IdentityError { Description = "User not found." });
            }
            var cacheKey = $"otp-{user.Id}";
            var otp = await cacheService.GetCachedDataAsync<string>(cacheKey);
            if (otp != request.OtpCode)
            {
                return IdentityResult.Failed(new IdentityError { Description = "Invalid OTP code." });
            }
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            var result = await userManager.ResetPasswordAsync(user, token, request.NewPassword);

            if (result.Succeeded)
            {
                await cacheService.RemoveCachedDataAsync(cacheKey);
                user.RefreshToken = null;
                user.RefreshTokenExpiryTime = null;
                await userManager.UpdateAsync(user);
            }

            return result;
        }

      
    }
}

