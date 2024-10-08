using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class CreateUpdateCategoryDto
    {
        public string Name { get; set; }    
        public string Description { get; set; } 
        public string ImageUrl { get; set; }

    }
}
