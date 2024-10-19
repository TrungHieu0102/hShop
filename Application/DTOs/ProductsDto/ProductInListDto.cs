using Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ProductsDto
{
    public class ProductInListDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string? Unit { get; set; }
        public ICollection<ProductImageDto> Images { get; set; } = new List<ProductImageDto>();
        public decimal Discount { get; set; }
        public DateTime DateCreated { get; set; }
        public int ViewCount { get; set; }

    }
}
