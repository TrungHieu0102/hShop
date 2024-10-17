using System;
using System.ComponentModel.DataAnnotations;

namespace Core.Entities
{
    public class Supplier
    {
        [Key]
        public Guid Id { get; set; }

        [Required(ErrorMessage = "Supplier name is required.")]
        [MaxLength(100, ErrorMessage = "Supplier name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [MaxLength(255, ErrorMessage = "Logo URL cannot exceed 255 characters.")]
        public string? Logo { get; set; }

        [MaxLength(15, ErrorMessage = "Phone number cannot exceed 15 characters.")]
        public string? PhoneNumber { get; set; }

        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [MaxLength(255, ErrorMessage = "Email cannot exceed 255 characters.")]
        public string? Email { get; set; }

        [MaxLength(255, ErrorMessage = "Address cannot exceed 255 characters.")]
        public string? Address { get; set; }

        [MaxLength(500, ErrorMessage = "Description cannot exceed 500 characters.")]
        public string? Description { get; set; }
    }
}
