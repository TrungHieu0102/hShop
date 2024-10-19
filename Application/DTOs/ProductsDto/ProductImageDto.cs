using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.ProductsDto
{
    public class ProductImageDto
    {
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsPrimary { get; set; } 
    }
}
