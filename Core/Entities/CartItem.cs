using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

public class CartItem
{
    [Key]
    public Guid Id { get; set; }
    public Guid CartId { get; set; }
    public Cart Cart { get; set; }
    
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
}