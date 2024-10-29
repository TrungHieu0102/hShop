
using Core.Entities;

namespace Core.Interfaces
{
    public interface IOrderRepository : IRepositoryBase<Order, Guid>
    {
        Task<Order> GetOrderByIdAsync(Guid orderId);
    }
}
