using Application.Interfaces;
using Core.Interfaces;
using Core.Model;

namespace Application.Services.Payment;

public class PaymentService : IPaymentService
{
    private readonly IUnitOfWorkBase _unitOfWork;
    private readonly ICartService _cartService;
    
    public PaymentService()
    {
        
    }
    public Task<Result<string>> ProcessPaymentAsync(Guid userId)
    {
        throw new NotImplementedException();
    }
}