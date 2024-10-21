﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.CategoriesDto
{
    public class CategoryInListDto
    {
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string ? Description { get; set; }
        public string? PictureUrl { get; set; } 

    }
}
