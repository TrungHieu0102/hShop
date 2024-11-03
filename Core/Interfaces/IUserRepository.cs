using Core.Entities;

namespace Core.Interfaces;

public interface IUserRepository: IRepositoryBase<User, Guid>
{
    Task RemoveUserFromRoles(Guid userId, string[] roles);
}