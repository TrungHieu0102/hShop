using Application.DTOs.AuthsDto;
using Application.DTOs.ProductsDto;
using Application.DTOs.UsersDto;
using Application.Interfaces;
using AutoMapper;
using Core.Entities;
using Core.Helpers;
using Core.Interfaces;
using Core.Model;
using Microsoft.AspNetCore.Identity;

namespace Application.Services
{

    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly IUnitOfWorkBase _unitOfWork;
        private readonly IEmailService _emailService;
        public UserService(UserManager<User> userManager, IMapper mapper, IUnitOfWorkBase unitOfWork, IEmailService emailService)
        {
            _userManager = userManager;
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _emailService = emailService;
        }

        public async Task<Result<UserInformatioResponeDto>> GetInformationByID(Guid id)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id.ToString());
                if (user == null)
                {
                    return new Result<UserInformatioResponeDto>
                    {
                        IsSuccess = false,
                        Message = "User not found",
                    };
                }
                var userDto = _mapper.Map<UserInformatioResponeDto>(user);
                return new Result<UserInformatioResponeDto>
                {
                    IsSuccess = true,
                    Data = userDto,
                };
            }
            catch (InvalidDataException)
            {
                return new Result<UserInformatioResponeDto>
                {
                    IsSuccess = false,
                    Message = "Invalid data",
                };
            }
        }
        public async Task<bool> DeleteUserById(Guid id)
        {
            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
            {
                return false;
            }
            var resutl = await _userManager.DeleteAsync(user);
            return resutl.Succeeded;
        }

        public async Task<Result<User>> UpdateUserAsync(Guid id, CreateUpdateUserDto userDto)
        {
            try
            {
                var user = await _userManager.FindByIdAsync(id.ToString());
                if (user == null)
                {
                    return new Result<User> { IsSuccess = false, Message = "User not found" };
                }
                if (user.Email != userDto.Email && userDto.Email != null)
                {
                    user.EmailConfirmed = false;
                    await _emailService.SendConfirmationEmail(userDto.Email, user);
                }
                var currentUserId = user.Id;
                _mapper.Map(userDto, user);
                user.Id = currentUserId;
                await _userManager.UpdateAsync(user);
                return new Result<User>
                {
                    IsSuccess = true
                };
            }
            catch (InvalidOperationException ex)
            {
                return new Result<User>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

    }
}

