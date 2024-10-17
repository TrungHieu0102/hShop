namespace Application.DTOs.ProductsDto
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public Guid SupplierId { get; set; }

    }
}
