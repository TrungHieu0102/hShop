using Application.DTOs;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<(bool,string)> SignUpAsync(SignUpDto signUpDto);
        Task<AuthResponseDto> SignInAsycn(SignInDto signInDto);
        Task SignOutAsync();
    }
}
