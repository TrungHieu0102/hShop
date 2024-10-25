using System.ComponentModel.DataAnnotations;

namespace Core.Entities;

public class Cart
{
    [Key]
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public User User { get; set; }
    public List<CartItem> Items { get; set; } = new List<CartItem>();
}