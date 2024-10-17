using Application.DTOs.AuthsDto;
using Application.Model;
using Core.Model;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

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
    }
}
