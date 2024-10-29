using Core.Entities;

namespace Application.DTOs.OrdersDto
{
    public class OrderDto
    {
 
        public Guid Id { get; set; }

        public Guid UserId { get; set; }
        
        public PaymentMethod? PaymentMethod { get; set; }

        public Ship? ShippingProvider { get; set; }

        public DateTime OrderDate { get; set; }

        public OrderStatus? OrderStatus { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
        public decimal TotalAmount { get; set; } 
        public decimal ShippingCost { get; set; }
        public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    }
}
