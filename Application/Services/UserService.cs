using Application.DTOs.UsersDto;
using Application.Interfaces;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Application.Model;
using Microsoft.EntityFrameworkCore;

namespace Application.Services
{
    public class UserService(
        UserManager<User> userManager,
        IMapper mapper,
        IUnitOfWorkBase unitOfWork,
        IEmailService emailService,
        ILogger<User> logger)
        : IUserService
    {
        public async Task<Result<UserInformatioResponeDto>> GetInformationById(Guid id)
        {
            try
            {
                var user = await userManager.FindByIdAsync(id.ToString());
                if (user == null)
                {
                    return new Result<UserInformatioResponeDto>
                    {
                        IsSuccess = false,
                        Message = "User not found",
                    };
                }

                var userDto = mapper.Map<UserInformatioResponeDto>(user);
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
            using var transaction = await unitOfWork.BeginTransactionAsync();
            try
            {
                var user = await userManager.FindByIdAsync(id.ToString());
                if (user == null)
                {
                    return false;
                }

                var result = await userManager.DeleteAsync(user);
                if (!result.Succeeded)
                {
                    return false;
                }

                await unitOfWork.CompleteAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                logger.LogError(ex, "Error deleting user with ID {UserId}: {Message}", id, ex.Message);
                return false;
            }
        }

        public async Task<Result<User>> UpdateUserAsync(Guid id, CreateUpdateUserDto userDto)
        {
            await using var transaction = await unitOfWork.BeginTransactionAsync();
            try
            {
                var user = await userManager.FindByIdAsync(id.ToString());
                if (user == null)
                {
                    return new Result<User> { IsSuccess = false, Message = "User not found" };
                }

                if (user.Email != userDto.Email && userDto.Email != null)
                {
                    user.EmailConfirmed = false;
                    await emailService.SendConfirmationEmail(userDto.Email, user);
                }

                var currentUserId = user.Id;
                mapper.Map(userDto, user);
                user.Id = currentUserId;
                await userManager.UpdateAsync(user);
                await unitOfWork.CompleteAsync();
                await transaction.CommitAsync();
                return new Result<User>
                {
                    IsSuccess = true
                };
            }
            catch (InvalidOperationException ex)
            {
                await transaction.RollbackAsync();
                return new Result<User>
                {
                    IsSuccess = false,
                    Message = ex.Message
                };
            }
        }

        public async Task<Result<User>> UpdateUserRoleAsync(Guid id, string[] role)
        {
            await using var transaction = await unitOfWork.BeginTransactionAsync();
            try
            {
                var user = await userManager.FindByIdAsync(id.ToString());
                if (user == null)
                {
                    throw new Exception("User not found");
                }

                var currentRoles = await userManager.GetRolesAsync(user);
                var rolesToAdd = role.Except(currentRoles).ToArray();
                var addedResult = await userManager.AddToRolesAsync(user, rolesToAdd);
                if (!addedResult.Succeeded)
                {
                    List<IdentityError> addedErrorList = addedResult.Errors.ToList();
                    var errorList = new List<IdentityError>();
                    errorList.AddRange(addedErrorList);
                    throw new Exception(string.Join("<br/>", errorList.Select(x => x.Description)));
                }

                await transaction.CommitAsync();
                return new Result<User>()
                {
                    IsSuccess = true,
                    Message = "User updated"
                };
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                return new Result<User>()
                {
                    IsSuccess = false,
                    Message = e.Message
                };
            }
        }

        public async Task<Result<User>> DeleteUserRolesAsync(Guid id, string[] rolesToDelete)
        {
            await using var transaction = await unitOfWork.BeginTransactionAsync();
            try
            {
                var user = await userManager.FindByIdAsync(id.ToString());

                if (user == null)
                {
                    throw new Exception("User  not found");
                }

                var userRoles = await userManager.GetRolesAsync(user);
                Console.WriteLine($"Current roles for user: {string.Join(", ", userRoles)}");
                var rolesToRemove = userRoles.Intersect(rolesToDelete).ToArray();
                await unitOfWork.Users.RemoveUserFromRoles(user.Id, rolesToRemove);
                await unitOfWork.CompleteAsync();
                await transaction.CommitAsync();
                return new Result<User>()
                {
                    IsSuccess = true,
                    Message = "User  roles updated"
                };
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                return new Result<User>()
                {
                    IsSuccess = false,
                    Message = e.Message
                };
            }
        }

        public async Task<PagedResult<UserDto>> GetAllUsersPaging(string? keyword, int pageIndex, int pageSize)
        {
            var query = userManager.Users;
            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(x => x.UserName != null && x.Email != null && x.PhoneNumber != null &&
                                         (x.FirstName.Contains(keyword)
                                          || x.UserName.Contains(keyword)
                                          || x.Email.Contains(keyword)
                                          || x.PhoneNumber.Contains(keyword)));
            }

            var totalRow = await query.CountAsync();
            query = query.OrderByDescending(x => x.UserName)
                .Skip((pageIndex - 1) * pageSize)
                .Take(pageSize);
            var pagedResponse = new PagedResult<UserDto>
            {
                IsSuccess = true,
                Results = await mapper.ProjectTo<UserDto>(query).ToListAsync(),
                CurrentPage = pageIndex,
                RowCount = totalRow,
                PageSize = pageSize
            };
            return pagedResponse;
        }

        public async Task<Result<User>> SetPassword(Guid id, SetPasswordRequest passwordRequest)
        {
            await using var transaction = await unitOfWork.BeginTransactionAsync();
            try
            {
                var user = await userManager.FindByIdAsync(id.ToString());
                if (user == null)
                {
                    throw new Exception("User not found");
                }

                user.PasswordHash = userManager.PasswordHasher.HashPassword(user, passwordRequest.NewPassword);
                var result = await userManager.UpdateAsync(user);
                await unitOfWork.CompleteAsync();
                await transaction.CommitAsync();
                if (!result.Succeeded)
                {
                    throw new Exception(string.Join("<br/>", result.Errors.Select(x => x.Description)));
                }

                return new Result<User>()
                {
                    IsSuccess = true,
                    Message = "User password updated"
                };
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                logger.LogError(e, "Error setting password");

                return new Result<User>()
                {
                    IsSuccess = true,
                    Message = e.Message
                };
            }
        }

        public async Task<Result<UserDto>> UpdateUser(Guid id, UpdateUserRequest userRequest)
        {
            await using var transaction = await unitOfWork.BeginTransactionAsync();
            try
            {
                var user = await userManager.FindByIdAsync(id.ToString());
                if (user == null)
                {
                    throw new Exception("User not found");
                }

                mapper.Map(userRequest, user);
                var result = await userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    throw new Exception(string.Join("<br/>", result.Errors.Select(x => x.Description)));
                }

                await unitOfWork.CompleteAsync();
                await transaction.CommitAsync();
                return new Result<UserDto>()
                {
                    IsSuccess = true,
                    Data = mapper.Map<UserDto>(user),
                };
            }
            catch (Exception e)
            {
                logger.LogError(e, "Error updating user");
                return new Result<UserDto>()
                {
                    IsSuccess = false,
                    Message = e.Message
                };
            }
        }
    }
}