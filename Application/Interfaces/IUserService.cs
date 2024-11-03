using Application.DTOs.ProductsDto;
using Application.DTOs.UsersDto;
using Core.Entities;
using Core.Model;
using Core.Model.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Application.Model;

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<Result<UserInformatioResponeDto>> GetInformationById(Guid id);
        Task<bool> DeleteUserById(Guid id);
        Task<Result<User>> UpdateUserAsync(Guid id, CreateUpdateUserDto userDto);
        Task<Result<User>> UpdateUserRoleAsync(Guid id, string[] role);
        Task<Result<User>> DeleteUserRolesAsync(Guid id, string[] rolesToDelete);
        Task<PagedResult<UserDto>> GetAllUsersPaging(string? keyword, int pageIndex, int pageSize);
        Task<Result<User>> SetPassword(Guid id, SetPasswordRequest passwordRequest);
        Task<Result<UserDto>> UpdateUser(Guid id, UpdateUserRequest userRequest);
    }
}
