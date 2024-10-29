using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

public class Order
{
    [Key]
    public Guid Id { get; set; }
    
    [Required]
    [ForeignKey("User")]
    public Guid UserId { get; set; }
    
    public required User User { get; set; } 
    
    [Required]
    [EnumDataType(typeof(PaymentMethod))]
    public PaymentMethod? PaymentMethod { get; set; }
    
    [EnumDataType(typeof(Ship))]
    public Ship? ShippingProvider { get; set; }
    
    [Required]
    public DateTime OrderDate { get; set; }
    
    [Required]
    [EnumDataType(typeof(OrderStatus))]
    public OrderStatus? OrderStatus { get; set; }
    
    [Required]
    [EnumDataType(typeof(PaymentStatus))]
    public PaymentStatus? PaymentStatus { get; set; }
    
    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Total amount must be non-negative.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; } 
    
    [Range(0, double.MaxValue, ErrorMessage = "Shipping cost must be non-negative.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal ShippingCost { get; set; }
    
    public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    
}

public enum OrderStatus
{
    Pending = 0,
    Confirmed = 1,
    Shipped = 2,
    Cancelled = 3
}

public enum PaymentMethod
{
    Paypal = 0,
    VnPay = 1,
    Cash = 2
}

public enum Ship
{
    None = 0,
    Grab = 1,
    JtExpress = 2,
}
public enum PaymentStatus
{
    Pending = 0,
    Completed = 1,
    Failed = 2,
    Refunded = 3
}