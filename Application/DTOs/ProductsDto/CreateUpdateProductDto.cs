using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ProductsDto
{
    public class CreateUpdateProductDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Description { get; set; } = string.Empty;
        public string Unit { get; set; } = string.Empty;
        public Guid CategoryId { get; set; }
        public Guid SupplierId { get; set; }
    }
}
