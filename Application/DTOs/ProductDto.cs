﻿
namespace Application.DTOs
{
    public class ProductDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public string Unit { get; set; }
        public string Slug { get; set; }    
        public Guid CategoryId { get; set; }
        public Guid SupplierId { get; set; }

    }
}
