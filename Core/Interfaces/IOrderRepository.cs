
using Core.Entities;

namespace Core.Interfaces
{
    public interface IOrderRepository : IRepositoryBase<Order, Guid>
    {
        Task<Order> GetOrderByIdAsync(Guid orderId);
        Task<Order> GetOrderById(Guid orderId);
        Task<IEnumerable<Order>> GetOrderByUserId(Guid userId);
        Task<IEnumerable<Order>> GetAllOrderAsync();

    }
}
