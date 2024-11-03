using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

public class OrderDetail
{
    // Composite Key Properties
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    [JsonIgnore]

    public required Order Order { get; set; } 
    public required Product Product { get; set; } 
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1.")]
    public int Quantity { get; set; }    
    [Required]
    [Range(0, double.MaxValue, ErrorMessage = "Unit price must be non-negative.")]
    [Column(TypeName = "decimal(18,2)")]
    public decimal UnitPrice { get; set; }  
    [NotMapped] // Calculated property, no need to map to DB
    public decimal TotalPrice => UnitPrice * Quantity; 
}