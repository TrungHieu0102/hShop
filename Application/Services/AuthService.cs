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
using Core.Model.Auth;
using static Google.Apis.Auth.GoogleJsonWebSignature;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Asn1.Ocsp;


namespace Application.Services
{
    public class AuthService(
        UserManager<User> userManager,
        IConfiguration configuration,
        SignInManager<User> signInManager,
        RoleManager<Role> roleManager,
        IEmailService emailService,
        ILogger<AuthService> logger,
        ICacheService cacheService)
        : IAuthService
    {
        public async Task<AuthResponseDto> SignInAsycn(SignInDto signInDto)
        {
            try
            {
                var user = await userManager.FindByEmailAsync(signInDto.Email);
                if (user != null)
                {
                    var loginProvider = await userManager.GetLoginsAsync(user);
                    if (loginProvider.Count > 0 && user.PasswordHash == null)
                    {
                        throw new Exception(
                            $"Please login by {loginProvider.First().ProviderDisplayName} or click forgotten password");
                    }
                }

                if (user == null || !await userManager.CheckPasswordAsync(user, signInDto.Password))
                {
                    throw new Exception("Wrong email or password");
                }

                var result = await signInManager.PasswordSignInAsync(signInDto.Email, signInDto.Password, false, false);
                var emailConfirmed = await userManager.IsEmailConfirmedAsync(user);
                if (!result.Succeeded)
                {
                    throw new Exception("Invalid Email or Password");
                }

                if (!emailConfirmed)
                {
                    await emailService.SendConfirmationEmail(signInDto.Email, user);
                    throw new Exception("Please confirm your email");
                }

                return await GenerateUserToken(user);
            }
            catch (Exception e)
            {
                logger.LogError(e.Message);
                return new AuthResponseDto()
                {
                    IsSuccess = false,
                    Message = e.Message
                };
            }
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

                currentUser = (await userManager.FindByEmailAsync(email))!;
            }
            else
            {
                currentUser = (await userManager.FindByEmailAsync(mail))!;
                if (currentUser == null)
                {
                    return IdentityResult.Failed(new IdentityError { Description = "User not found." });
                }
            }

            var otp = PasswordExtensions.GenerateOtp();
            var cacheKey = $"otp-{currentUser.Id}";
            await cacheService.SetCachedDataAsyncWithTime(cacheKey, otp, TimeSpan.FromMinutes(5));
            string body = GenerateEmailBody.GetEmailOTPBody(mail, otp);

            await emailService.SendEmailAsync(currentUser.Email!, "Mã xác nhận đổi mật khẩu", body, true);

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

        public async Task<AuthResponseDto> AuthenticateGoogleUserAsync(GoogleUserRequest request)
        {
            try
            {
                Payload payload = await ValidateAsync(request.idToken, new ValidationSettings
                {
                    Audience = new[] { configuration["Authentication:Google:ClientId"] }
                });

                var user = await GetOrCreateExternalLoginUser(GoogleUserRequest.PROVIDER, payload.Subject,
                    payload.Email, payload.GivenName, payload.FamilyName);
                return await GenerateUserToken(user);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error during Google user authentication for token: {Token}", request.idToken);
                throw new Exception("Authentication failed. Please try again later.");
            }
        }

        #region Private Methods

        private async Task<List<string>> GetPermissionAsync(string userId)
        {
            var user = await userManager.FindByIdAsync(userId);
            var roles = await userManager.GetRolesAsync(user!);
            if (!roles.Any())
            {
                roles.Add("User");
            }

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
                    var claims = await roleManager.GetClaimsAsync(role!);
                    var roleClaimValues = claims.Select(x => x.Value).ToList();
                    permissions.AddRange(roleClaimValues);
                }
            }
            return permissions.Distinct().ToList();
        }

        private async Task<User> GetOrCreateExternalLoginUser(string provider, string key, string email,
            string firstName, string lastName)
        {
            try
            {
                var user = await userManager.FindByLoginAsync(provider, key);
                if (user != null)
                    return user;

                user = await userManager.FindByEmailAsync(email);
                Guid id = Guid.NewGuid();

                if (user == null)
                {
                    user = new User
                    {
                        Email = email,
                        UserName = email,
                        FirstName = firstName,
                        LastName = lastName,
                        Id = id,
                        EmailConfirmed = true
                    };

                    var createResult = await userManager.CreateAsync(user);
                    if (!createResult.Succeeded)
                    {
                        throw new Exception("User  creation failed: " +
                                            string.Join(", ", createResult.Errors.Select(x => x.Description)));
                    }

                    var info = new UserLoginInfo(provider, key, provider.ToUpperInvariant());
                    var result = await userManager.AddLoginAsync(user, info);

                    if (!result.Succeeded)
                    {
                        throw new Exception("Adding login failed: " +
                                            string.Join(", ", result.Errors.Select(x => x.Description)));
                    }
                }

                return user;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error in GetOrCreateExternalLoginUser  for provider: {Provider}, key: {Key}",
                    provider, key);
                throw;
            }
        }

        private async Task<AuthResponseDto> GenerateUserToken(User user)
        {
            try
            {
                var roles = await userManager.GetRolesAsync(user);
                if (!roles.Any())
                {
                    await userManager.AddToRoleAsync(user, "User");
                }

                var permission = await GetPermissionAsync(user.Id.ToString());
                var authClaims = new List<Claim>
                {
                    new(JwtRegisteredClaimNames.Email, user.Email!),
                    new Claim(UserClaims.Id, user.Id.ToString()),
                    new Claim(ClaimTypes.NameIdentifier, user.Email!),
                    new Claim(ClaimTypes.Name, user.UserName!),
                    new Claim(UserClaims.FirstName, user.FirstName),
                    new Claim(UserClaims.Roles, string.Join(";", roles)),
                    new Claim(UserClaims.Permissions, JsonSerializer.Serialize(permission)),
                    new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };

                authClaims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));

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
                    expires: DateTime.Now.AddDays(1),
                    signingCredentials: new SigningCredentials(new SymmetricSecurityKey(authKey),
                        SecurityAlgorithms.HmacSha256)
                );

                var refreshToken = RefreshToken.GenerateRefreshToken();
                await SaveRefreshToken(user.Id, refreshToken);

                return new AuthResponseDto
                {
                    AccessToken = new JwtSecurityTokenHandler().WriteToken(token),
                    RefreshToken = user.RefreshToken,
                    Expiration = user.RefreshTokenExpiryTime ?? DateTime.MinValue,
                    IsSuccess = true,
                    Message = "Success"
                };
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error generating token for user: {User Id}", user.Id);
                throw new Exception("Token generation failed. Please try again later.");
            }
        }

        #endregion
    }
}