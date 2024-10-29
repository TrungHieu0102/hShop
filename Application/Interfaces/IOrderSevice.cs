using Application.DTOs.OrdersDto;
using Core.Entities;
using Core.Model;

namespace Application.Interfaces;

public interface IOrderSevice
{
    Task<Result<OrderDto>> CreateOrderAsync(Guid userId, CreateUpdateOrderDto createUpdateOrderDto);
}