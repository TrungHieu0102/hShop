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

namespace Application.Interfaces
{
    public interface IUserService
    {
        Task<Result<UserInformatioResponeDto>> GetInformationByID(Guid id);
        Task<bool> DeleteUserById(Guid id);
        Task<Result<User>> UpdateUserAsync(Guid id, CreateUpdateUserDto userDto);
    }
}
