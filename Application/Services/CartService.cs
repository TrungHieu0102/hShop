    using Application.Interfaces;
    using Core.Entities;
    using Core.Interfaces;
    using Core.Model;
    using Microsoft.Extensions.Logging;

    namespace Application.Services;

    public class CartService : ICartService
    {
        private readonly IUnitOfWorkBase _unitOfWork;
        private readonly ILogger<Cart> _logger;

        public CartService(IUnitOfWorkBase unitOfWork, ILogger<Cart> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<Cart>> AddToCartAsync(Guid userId, Guid productId, int quantity)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var product = await _unitOfWork.Products.GetByIdAsync(productId);
                if (product == null || product.Quantity < quantity)
                {
                    return new Result<Cart>()
                    {
                        IsSuccess = false,
                        Message = "Sản phẩm không tồn tại hoặc không đủ số lượng"
                    };
                }

                var cart = await _unitOfWork.Carts.GetCart(userId);
                if (cart == null)
                {
                    cart = new Cart { UserId = userId };
                    await _unitOfWork.Carts.CreateCartAsync(cart);
                }

                var cartItem = cart.Items.FirstOrDefault(x => x.ProductId == productId);
                if (cartItem != null)
                {
                    cartItem.Quantity += quantity;
                    await _unitOfWork.Carts.UpdateCartItemAsync(cartItem);
                }
                else
                {
                    // Tạo mới cart item
                    var newCartItem = new CartItem
                    {
                        CartId = cart.Id,
                        ProductId = productId,
                        Quantity = quantity
                    };
                    await _unitOfWork.Carts.AddCartItemAsync(newCartItem);
                }

                await transaction.CommitAsync();
                return new Result<Cart>()
                {
                    IsSuccess = true,
                    Message = "Thêm sản phẩm thành công"
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                await transaction.RollbackAsync();
                return new Result<Cart>()
                {
                    IsSuccess = false,
                    Message = e.Message
                };
            }

        }
        
        public async Task<Result<Cart>> RemoveFromCartAsync(Guid userId, Guid productId)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var cart = await _unitOfWork.Carts.GetCart(userId);
                if (cart == null)
                {
                    return new Result<Cart>()
                    {
                        IsSuccess = false,
                        Message = "Cart not found"
                    };
                }
                var cartItem = cart.Items.FirstOrDefault(x => x.ProductId == productId);
                if (cartItem == null)
                {
                    return new Result<Cart>()
                    {
                        IsSuccess = false,
                        Message = "Cart item not found"
                    };
                }

                await _unitOfWork.Carts.RemoveCartItemAsync(cartItem.Id);
                await transaction.CommitAsync();
                return new Result<Cart>()
                {
                    IsSuccess = true,

                };
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                await transaction.RollbackAsync();
                return new Result<Cart>()
                {
                    IsSuccess = false,
                    Message = e.Message
                };
            }
        }

        public async Task<Result<Cart>> ClearCartAsync(Guid userId)
        {
            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                var cart = await _unitOfWork.Carts.GetCart(userId);
                if (cart == null)
                {
                    return new Result<Cart>()
                    {
                        IsSuccess = false,
                        Message = "Cart not found"
                    };
                }

                await _unitOfWork.Carts.ClearCartAsync(cart.Id);
                await transaction.CommitAsync();
                return new Result<Cart>()
                {
                    IsSuccess = true,
                };
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                await transaction.RollbackAsync();
                return new Result<Cart>()
                {
                    IsSuccess = false,
                    Message = e.Message
                };
            }



        }
    }