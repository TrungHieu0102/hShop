using Application.DTOs.OrdersDto;
using Application.Interfaces;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Core.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class OrderService : IOrderSevice
{
    private readonly IUnitOfWorkBase _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<OrderService> _logger;
    private readonly UserManager<User> _userManager;
    private readonly ICartService _cartService;

    public OrderService(IUnitOfWorkBase unitOfWork, IMapper mapper, ILogger<OrderService> logger,
        UserManager<User> userManager, ICartService cartService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
        _userManager = userManager;
        _cartService = cartService;
    }

    public async Task<Result<OrderDto>> CreateOrderAsync(Guid userId, CreateUpdateOrderDto createUpdateOrderDto)
    {
      await using var transaction = await _unitOfWork.BeginTransactionAsync();
      try
      {
          var cart = await _unitOfWork.Carts.GetCartWithItemsAsync(userId);
          if (cart == null || !cart.Items.Any())
          {
              return new Result<OrderDto>
              {
                  IsSuccess = false,
                  Message = "Giỏ hàng rỗng hoặc không tồn tại."
              };
          }

          var user = await _userManager.FindByIdAsync(userId.ToString());
          if (user == null)
          {
              return new Result<OrderDto>
              {
                  IsSuccess = false,
                  Message = "Người dùng không tồn tại."
              };
          }

          var order = _mapper.Map<Order>(createUpdateOrderDto);
          order.Id = Guid.NewGuid();
          order.UserId = userId;
          order.User = user;
          order.OrderDate = DateTime.UtcNow;
          order.OrderStatus = OrderStatus.Pending;
          order.PaymentStatus = PaymentStatus.Pending;
          order.OrderDetails = new List<OrderDetail>();
          foreach (var item in cart.Items)
          {
              var product = await _unitOfWork.Products.GetByIdAsync(item.ProductId);
              if (product != null)
              {
                  order.OrderDetails.Add(new OrderDetail
                  {
                      Order = order,
                      ProductId = item.ProductId,
                      Quantity = item.Quantity,
                      UnitPrice = item.UnitPrice,
                      Product = product
                  });
              }
          }

          _unitOfWork.Orders.Add(order);
          await _unitOfWork.CompleteAsync();
          var orderDto = _mapper.Map<OrderDto>(order);
          await _cartService.ClearCartAsync(userId);
          await transaction.CommitAsync();
          return new Result<OrderDto>
          {
              IsSuccess = true,
              Message = "Đơn hàng đã được tạo thành công.",
              Data = orderDto
          };
      }
      catch (Exception e)
      {
         _logger.LogError(e.Message);
         return new Result<OrderDto>()
         {
             IsSuccess = false,
             Message = e.Message
         };
      }
    }
}