using Core.Entities;

namespace Application.DTOs.OrdersDto
{
    public class OrderDto
    {
        public Guid Id { get; init; }
        public Guid UserId { get; init; }
        public string? PaymentMethod { get; init; }  // Automatically converts enum to string
        public string? ShippingProvider { get; init; }
        public DateTime OrderDate { get; init; }
        public string? OrderStatus { get; init; }
        public string? PaymentStatus { get; init; }
        public decimal TotalAmount { get; init; }
        public decimal ShippingCost { get; init; }
        public List<OrderDetailDto> OrderDetails { get; init; } = new List<OrderDetailDto>();
    }
    
}
