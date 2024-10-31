using Application.DTOs.AuthsDto;
using Application.Model;
using Core.Entities;
using Core.Model.Auth;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<(bool, string)> SignUpAsync(SignUpDto signUpDto);
        Task<AuthResponseDto> SignInAsycn(SignInDto signInDto);
        Task SignOutAsync();
        Task<string> ConfirmEmail(string userId, string token);

        Task<IdentityResult> RequestPasswordChangeAsync(string mail, ClaimsPrincipal user);
        Task<IdentityResult> ConfirmPasswordChangeAsync(ConfirmPasswordChangeRequest request);
        Task<AuthResponseDto> AuthenticateGoogleUserAsync(GoogleUserRequest request);
    }
}
