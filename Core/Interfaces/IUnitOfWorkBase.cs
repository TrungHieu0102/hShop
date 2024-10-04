
namespace Core.Interfaces
{
    public interface IUnitOfWorkBase : IDisposable
    {
        Task SaveChangesAsync();
    }

}
