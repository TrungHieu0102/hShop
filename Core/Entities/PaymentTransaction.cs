using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace Core.Entities;
public class PaymentTransaction
{
    [Key] public Guid TransactionId { get; set; } 
    [Required] 
    [ForeignKey("Order")]
    public Guid OrderId { get; set; }
    public virtual Order Order { get; set; }
    [Required] 
    [ForeignKey("User ")]
    public Guid UserId { get; set; }
    public virtual User User { get; set; }
    [Required]
    public DateTime PaymentDate { get; set; } = DateTime.Now;
    [Required] 
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
    public decimal Amount { get; set; }
    [Required] 
    [StringLength(50, ErrorMessage = "Payment method cannot exceed 50 characters.")]
    public PaymentMethod PaymentMethod { get; set; }
    [StringLength(100, ErrorMessage = "Transaction reference cannot exceed 100 characters.")]
    public string TransactionReference { get; set; }
    [StringLength(20, ErrorMessage = "Status cannot exceed 20 characters.")]
    public string Status { get; set; }
    [StringLength(10, ErrorMessage = "Currency cannot exceed 10 characters.")]
    public string Currency { get; set; }
    [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters.")]
    public string? Notes { get; set; }
}