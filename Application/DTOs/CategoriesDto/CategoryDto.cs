﻿
using Application;

namespace Application.DTOs.CategoriesDto
{
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Slug { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? PictureUrl { get; set; }
    }
}
