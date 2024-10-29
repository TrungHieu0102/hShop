using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Core.Entities;

namespace Application.DTOs.OrdersDto;

public class CreateUpdateOrderDto
{
    [Required]
    [EnumDataType(typeof(PaymentMethod))]
    public PaymentMethod? PaymentMethod { get; set; }
    [EnumDataType(typeof(Ship))]
    public Ship? ShippingProvider { get; set; }
    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Total amount must be non-negative.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal TotalAmount { get; set; } 
    [Range(0, double.MaxValue, ErrorMessage = "Shipping cost must be non-negative.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal ShippingCost { get; set; }
}