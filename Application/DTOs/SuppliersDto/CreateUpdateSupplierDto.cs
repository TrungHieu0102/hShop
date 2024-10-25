﻿using Microsoft.AspNetCore.Http;

namespace Application.DTOs.SuppliersDto
{
    public class CreateUpdateSupplierDto
    {
        public string Name { get; set; } = string.Empty;
        public IFormFile? Logo { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Description { get; set; }
    }
}
