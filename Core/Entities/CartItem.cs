using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities;

public class CartItem
{
    public Guid CartId { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    [ForeignKey(nameof(CartId))]
    public Cart Cart { get; set; }

    [ForeignKey(nameof(ProductId))]
    public Product Product { get; set; }
}