using Core.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Product
{
    [Key]
    public Guid Id { get; set; }
  
    [MaxLength(100)]
    [Required(ErrorMessage = "Product name is required.")]
    public string Name { get; set; } = string.Empty;

    [MaxLength(120)]
    [Required(ErrorMessage = "Slug is required.")]
    public string Slug { get; set; } = string.Empty;

    [MaxLength(255)]
    public string Description { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18, 2)")]
    [Required(ErrorMessage = "Price is required.")]
    [Range(0, double.MaxValue, ErrorMessage = "Price must be a positive value.")]
    public decimal Price { get; set; }

    [MaxLength(20)]
    [Required(ErrorMessage = "Unit is required.")]
    public string Unit { get; set; } = string.Empty;

    [Column(TypeName = "decimal(18, 2)")]
    [Range(0, 100, ErrorMessage = "Discount must be between 0 and 100.")]
    public decimal? Discount { get; set; }
    [Range(0,10000, ErrorMessage = "Price must be between 0 and 10000.")]
    public int? Quantity { get; set; }
    [Required]
    public DateTime DateCreated { get; set; } = DateTime.Now;

    [Range(0, int.MaxValue, ErrorMessage = "View count must be a positive value.")]
    public int? ViewCount { get; set; }
    
    [Required]
    public Guid CategoryId { get; set; }

    [ForeignKey(nameof(CategoryId))]
    public Category Category { get; set; }

    [Required]
    public Guid SupplierId { get; set; }

    [ForeignKey(nameof(SupplierId))]
    public Supplier Supplier { get; set; }

    [MaxLength(10)]
    public ICollection<ProductImage> Images { get; set; }
    [Timestamp]
    public byte[] RowVersion { get; set; }
    public List<OrderDetail> OrderDetails { get; set; } = new List<OrderDetail>();
    public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();

    
}
