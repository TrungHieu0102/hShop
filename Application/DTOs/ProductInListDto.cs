using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#nullable disable

namespace Application.DTOs
{
    public class ProductInListDto
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string Slug { get; set; }

        public string Description { get; set; }
        public decimal Price { get; set; }

        public string Unit { get; set; }
        public string PictureUrl { get; set; }
        public decimal Discount { get; set; }
        public DateOnly DateCreated { get; set; }
        public int ViewCount { get; set; }

    }
}
