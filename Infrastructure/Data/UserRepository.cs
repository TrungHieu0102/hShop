using Core.Entities;
using Core.Interfaces;
using Infrastructure.Data.Context;
using Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Data;

public class UserRepository(HshopContext context) : RepositoryBase<User, Guid>(context), IUserRepository
{
    public async Task RemoveUserFromRoles(Guid userId, string[] roleNames)
    {
        if (roleNames == null || roleNames.Length == 0)
            return;
        foreach (var roleName in roleNames)
        {
            var role = await _context.Roles.FirstOrDefaultAsync(x => x.Name == roleName);
            if (role == null)
            {
                return;
            }
            var userRole = await _context.UserRoles.FirstOrDefaultAsync(x => x.RoleId == role.Id && x.UserId == userId);
            if (userRole == null)
            {
                return;
            }
            _context.UserRoles.Remove(userRole);
        }
    }
}