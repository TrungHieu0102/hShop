using System.Security.Claims;
using Application.DTOs.OrdersDto;
using Core.Entities;
using Core.Model;

namespace Application.Interfaces;

public interface IOrderService
{
    Task<Result<OrderDto>> CreateOrderAsync(Guid userId, CreateUpdateOrderDto createUpdateOrderDto);
    Task<Result<OrderDto>> GetOrderAsync(Guid orderId);
    Task<PagedResult<OrderDto>> GetOrderByUserIdAsync(Guid userId, int page, int pageSize, bool isDescending);
    Task<PagedResult<OrderDto>> GetOrderByUserClaimAsync(Guid userId,int page, int pageSize, bool isDescending);
    Task<PagedResult<OrderDto>> GetAllOrderAsync(int page, int pageSize, bool isDescending);
    Task<Result<OrderDto>> UpdatePaymentStatusAsync(Guid orderId, PaymentStatus newStatus);
    Task<Result<OrderDto>> UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus);
    Task<PagedResult<OrderDto>> GetOrdersByStatusAsync(Guid userId, OrderStatus orderStatus, int page, int pageSize,
        bool isDescending);
    Task<Result<OrderDto>> UpdateOrderAsync(Guid id, CreateUpdateOrderDto requestOrder);
    Task<Result<OrderDto>> DeleteAllOrderCanceled();
}