
using Core.Entities;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Core.Interfaces
{
    public interface IOrderRepository : IRepositoryBase<Order, Guid>
    {
        Task<Order> GetOrderById(Guid orderId);
        Task<IEnumerable<Order>> GetOrderByUserId(Guid userId);
        Task<IEnumerable<Order>> GetAllOrderAsync();
        Task<Order> GetOrderByIdInclude(Guid orderId);
        void Attach(Order order);
        EntityEntry<Order> Entry(Order order);

    }
}
