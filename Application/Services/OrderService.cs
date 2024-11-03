using System.Text.RegularExpressions;
using Application.Common.TemplateEmail;
using Application.DTOs.OrdersDto;
using Application.Interfaces;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class OrderService(
    IUnitOfWorkBase unitOfWork,
    IMapper mapper,
    ILogger<OrderService> logger,
    ICacheService cacheService,
    IEmailService emailService,
    UserManager<User> userManager)
    : IOrderService
{
    public async Task<Result<OrderDto>> CreateOrderAsync(Guid userId, CreateUpdateOrderDto createUpdateOrderDto)
    {
        await using var transaction = await unitOfWork.BeginTransactionAsync();
        try
        {
            var cart = await unitOfWork.Carts.GetCartWithItemsAsync(userId);
            if (cart == null || cart.Items.Count == 0)
            {
                return new Result<OrderDto>
                {
                    IsSuccess = false,
                    Message = "Cart is empty"
                };
            }

            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return new Result<OrderDto>
                {
                    IsSuccess = false,
                    Message = "User does not exists"
                };
            }

            var order = mapper.Map<Order>(createUpdateOrderDto);
            order.Id = Guid.NewGuid();
            order.UserId = userId;
            order.User = user;
            order.OrderDate = DateTime.UtcNow;
            order.OrderStatus = OrderStatus.Pending;
            order.PaymentStatus = PaymentStatus.Pending;
            order.OrderDetails = new List<OrderDetail>();
            var totalAmount = (decimal)0;
            foreach (var item in cart.Items)
            {
                var product = await unitOfWork.Products.GetByIdAsync(item.ProductId);
                if (product != null)
                {
                    order.OrderDetails.Add(new OrderDetail
                    {
                        Order = order,
                        ProductId = item.ProductId,
                        ProductName = product.Name,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        Product = product
                    });
                    totalAmount += item.UnitPrice;
                }
            }

            order.TotalAmount = totalAmount;
            unitOfWork.Orders.Add(order);
            await unitOfWork.CompleteAsync();
            var orderDto = mapper.Map<OrderDto>(order);
            await unitOfWork.Carts.ClearCartAsync(cart.Id);
            await transaction.CommitAsync();
            return new Result<OrderDto>
            {
                IsSuccess = true,
                Message = "Create order successfully",
                Data = orderDto
            };
        }
        catch (Exception e)
        {
            logger.LogError("An error occurred while creating the order: {ErrorMessage}", e.Message);
            return new Result<OrderDto>()
            {
                IsSuccess = false,
                Message = e.Message
            };
        }
    }

    public async Task<Result<OrderDto>> GetOrderAsync(Guid orderId)
    {
        var cacheKey = $"Order-{orderId}";
        var orderInCache = await cacheService.GetCachedDataAsync<OrderDto>(cacheKey);
        var order = mapper.Map<Order>(orderInCache) ?? await unitOfWork.Orders.GetOrderById(orderId);
        if (order == null)
        {
            return new Result<OrderDto>()
            {
                IsSuccess = false,
                Message = "Order does not exists"
            };
        }

        var orderDto = mapper.Map<OrderDto>(order);
        await cacheService.SetCachedDataAsync(cacheKey, orderDto);
        return new Result<OrderDto>()
        {
            IsSuccess = true,
            Data = orderDto
        };
    }

    public async Task<PagedResult<OrderDto>> GetOrderByUserIdAsync(Guid userId, int page, int pageSize,
        bool isDescending)
    {
        try
        {
            var orders = await unitOfWork.Orders.GetOrderByUserId(userId);

            orders = isDescending
                ? orders.OrderByDescending(o => o.OrderDate)
                : orders.OrderBy(o => o.OrderDate);

            var totalRows = orders.Count();
            var pagedOrder = orders.Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            return new PagedResult<OrderDto>()
            {
                CurrentPage = page,
                PageSize = pageSize,
                RowCount = totalRows,
                Results = mapper.Map<IEnumerable<OrderDto>>(pagedOrder),
                IsSuccess = true
            };
        }
        catch (Exception e)
        {
            logger.LogError("An error occurred while creating the order: {ErrorMessage}", e.Message);
            return new PagedResult<OrderDto>
            {
                CurrentPage = page,
                PageSize = pageSize,
                RowCount = 0,
                Results = [],
                IsSuccess = false
            };
        }
    }

    public async Task<PagedResult<OrderDto>> GetOrderByUserClaimAsync(Guid userId, int page, int pageSize,
        bool isDescending)
    {
        try
        {
            var orders = await unitOfWork.Orders.GetOrderByUserId(userId);

            orders = isDescending ? orders.OrderByDescending(o => o.OrderDate) : orders.OrderBy(o => o.OrderDate);
            var totalRows = orders.Count();
            var pagedOrder = orders.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return new PagedResult<OrderDto>()
            {
                CurrentPage = page,
                PageSize = pageSize,
                RowCount = totalRows,
                Results = mapper.Map<IEnumerable<OrderDto>>(pagedOrder),
                IsSuccess = true
            };
        }
        catch (Exception e)
        {
            logger.LogError("An error occurred while creating the order: {ErrorMessage}", e.Message);
            return new PagedResult<OrderDto>
            {
                CurrentPage = page,
                PageSize = pageSize,
                RowCount = 0,
                Results = [],
                IsSuccess = false
            };
        }
    }

    public async Task<PagedResult<OrderDto>> GetAllOrderAsync(int page, int pageSize, bool isDescending)
    {
        try
        {
            var orders = await unitOfWork.Orders.GetAllOrderAsync();
            orders = isDescending
                ? orders.OrderByDescending(o => o.OrderDate)
                : orders.OrderBy(o => o.OrderDate);

            var totalRows = orders.Count();

            var pagedOrder = orders.Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();
            return new PagedResult<OrderDto>()
            {
                CurrentPage = page,
                PageSize = pageSize,
                RowCount = totalRows,
                Results = mapper.Map<IEnumerable<OrderDto>>(pagedOrder),
                IsSuccess = true
            };
        }
        catch (Exception e)
        {
            logger.LogError("An error occurred while creating the order: {ErrorMessage}", e.Message);
            return new PagedResult<OrderDto>
            {
                CurrentPage = page,
                PageSize = pageSize,
                RowCount = 0,
                Results = [],
                IsSuccess = false
            };
        }
    }

    public async Task<Result<OrderDto>> UpdatePaymentStatusAsync(Guid orderId, PaymentStatus newStatus)
    {
        await using var transaction = await unitOfWork.BeginTransactionAsync();
        try
        {
            var cacheKey = $"Order-{orderId}";
            var orderInCache = await cacheService.GetCachedDataAsync<OrderDto>(cacheKey);
            var order = mapper.Map<Order>(orderInCache) ?? await unitOfWork.Orders.GetByIdAsync(orderId);
            if (order == null)
            {
                return new Result<OrderDto>
                {
                    IsSuccess = false,
                    Message = "Order not found"
                };
            }

            const string regex = "^[0-3]$";
            if (!Regex.IsMatch(((int)newStatus).ToString(), regex))
            {
                return new Result<OrderDto>()
                {
                    IsSuccess = false,
                    Message = "Invalid payment status (0-3)"
                };
            }

            if (order.PaymentStatus == newStatus)
            {
                return new Result<OrderDto>()
                {
                    IsSuccess = false,
                    Message = "The payment status is the same as the current status"
                };
            }

            order.PaymentStatus = newStatus;

            unitOfWork.Orders.Update(order);
            await unitOfWork.CompleteAsync();
            await transaction.CommitAsync();
            await cacheService.RemoveCachedDataAsync(cacheKey);
            var orderDto = mapper.Map<OrderDto>(order);
            return new Result<OrderDto>
            {
                IsSuccess = true,
                Data = orderDto,
                Message = "Payment status updated successfully"
            };
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            logger.LogError("An error occurred while updating the order: {ErrorMessage}", e.Message);
            return new Result<OrderDto>
            {
                IsSuccess = false,
                Message = "Order not found"
            };
        }
    }

    public async Task<Result<OrderDto>> UpdateOrderStatusAsync(Guid orderId, OrderStatus newStatus)
    {
        await using var transaction = await unitOfWork.BeginTransactionAsync();
        try
        {
            var cacheKey = $"Order-{orderId}";
            var orderInCache = await cacheService.GetCachedDataAsync<OrderDto>(cacheKey);
            var order = mapper.Map<Order>(orderInCache) ?? await unitOfWork.Orders.GetOrderByIdInclude(orderId);
            if (order == null)
            {
                return new Result<OrderDto>
                {
                    IsSuccess = false,
                    Message = "Order not found"
                };
            }

            const string regex = "^[0-3]$";
            if (!Regex.IsMatch(((int)newStatus).ToString(), regex))
            {
                return new Result<OrderDto>()
                {
                    IsSuccess = false,
                    Message = "Invalid order status (0-3)"
                };
            }

            if (order.OrderStatus == newStatus)
            {
                return new Result<OrderDto>()
                {
                    IsSuccess = false,
                    Message = "The order status is the same as the current status"
                };
            }

            if (newStatus == OrderStatus.Shipped)
            {
                var user = await userManager.FindByIdAsync(order.UserId.ToString());
                var orderItem = await GetOrderItemsAsync(order);
                string body =
                    GenerateEmailBody.GetEmailOrderStatusBody(user.GetFullName(), orderId, orderItem,
                        order.TotalAmount);

                await emailService.SendEmailAsync(user.Email, "Đơn hàng đã được giao thành công",
                    body, true);
            }

            order.OrderStatus = newStatus;
            unitOfWork.Orders.Update(order);
            await unitOfWork.CompleteAsync();
            await transaction.CommitAsync();
            await cacheService.RemoveCachedDataAsync(cacheKey);
            var orderDto = mapper.Map<OrderDto>(order);
            return new Result<OrderDto>
            {
                IsSuccess = true,
                Data = orderDto,
                Message = "Order status updated successfully"
            };
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            logger.LogError("An error occurred while updating the order: {ErrorMessage}", e.Message);
            return new Result<OrderDto>
            {
                IsSuccess = false,
                Message = "Order not found"
            };
        }
    }

    public async Task<PagedResult<OrderDto>> GetOrdersByStatusAsync(Guid userId, OrderStatus orderStatus, int page,
        int pageSize, bool isDescending)
    {
        var orders = await unitOfWork.Orders.GetOrderByUserId(userId);
        if (!orders.Any())
        {
            return new PagedResult<OrderDto>
            {
                CurrentPage = page,
                PageSize = pageSize,
                RowCount = 0,
                Results = [],
                IsSuccess = false
            };
        }

        orders = isDescending
            ? orders.OrderByDescending(o => o.OrderDate)
            : orders.OrderBy(o => o.OrderDate);

        var totalRows = orders.Count();

        var pagedOrder = orders.Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
        var order = orders.Where(o => o.OrderStatus == orderStatus).ToList();
        return new PagedResult<OrderDto>
        {
            CurrentPage = page,
            PageSize = pageSize,
            RowCount = totalRows,
            Results = mapper.Map<IEnumerable<OrderDto>>(order),
            IsSuccess = true
        };
    }

    public async Task<Result<OrderDto>> UpdateOrderAsync(Guid id, CreateUpdateOrderDto requestOrder)
    {
        await using var transaction = await unitOfWork.BeginTransactionAsync();
        try
        {
            var cacheKey = $"Order-{id}";
            var orderInCache = await cacheService.GetCachedDataAsync<OrderDto>(cacheKey);
            var order = mapper.Map<Order>(orderInCache) ?? await unitOfWork.Orders.GetByIdAsync(id);
            if (order == null)
            {
                return new Result<OrderDto>
                {
                    IsSuccess = false,
                    Message = "Order not found"
                };
            }

            if (orderInCache != null)
            {
                await cacheService.RemoveCachedDataAsync(cacheKey);
            }

            order.PaymentMethod = requestOrder.PaymentMethod;
            order.ShippingProvider = requestOrder.ShippingProvider;
            order.TotalAmount = requestOrder.TotalAmount;
            order.ShippingCost = requestOrder.ShippingCost;
            unitOfWork.Orders.Update(order);
            await unitOfWork.CompleteAsync();
            await transaction.CommitAsync();
            return new Result<OrderDto>
            {
                IsSuccess = true,
                Message = "Update order successfully",
                Data = mapper.Map<OrderDto>(order)
            };
        }
        catch (Exception e)
        {
            await transaction.RollbackAsync();
            logger.LogError("An error occurred while updating the order: {ErrorMessage}", e.Message);
            return new Result<OrderDto>
            {
                IsSuccess = false,
                Message = "Order not found"
            };
        }
    }

    public async Task<Result<OrderDto>> DeleteAllOrderCanceled()
    {
        await using var transaction = await unitOfWork.BeginTransactionAsync();
        try
        {
            var order = await unitOfWork.Orders.GetAllOrderAsync();
            order = order.Where(o => o.OrderStatus == OrderStatus.Cancelled).ToList();
            unitOfWork.Orders.RemoveRange(order);
            await unitOfWork.CompleteAsync();
            await transaction.CommitAsync();
            return new Result<OrderDto>()
            {
                IsSuccess = true,
                Message = "Delete all order canceled successfully"
            };
        }
        catch (Exception e)
        {
            logger.LogError("An error occurred while updating the order: {ErrorMessage}", e.Message);
            return new Result<OrderDto>()
            {
                IsSuccess = false,
                Message = e.Message
            };
        }
    }

    private static async Task<List<OrderDetailDto>> GetOrderItemsAsync(Order order)
    {
        var orderItems = order.OrderDetails.Select(detail => new OrderDetailDto()
        {
            ProductName = detail.ProductName,
            Quantity = detail.Quantity,
            UnitPrice = detail.UnitPrice
        }).ToList();
        return await Task.FromResult(orderItems);
    }
}