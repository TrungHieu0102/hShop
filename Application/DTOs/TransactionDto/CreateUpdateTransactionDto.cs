using Core.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.TransactionDto
{
    public class CreateUpdateTransactionDto
    {
        [ForeignKey("Order")]
        public Guid OrderId { get; set; }
        [Required]
        [ForeignKey("User ")]
        public Guid UserId { get; set; }
        [Required]
        public DateTime PaymentDate { get; set; }
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero.")]
        public decimal Amount { get; set; }
        [Required]
        [StringLength(50, ErrorMessage = "Payment method cannot exceed 50 characters.")]
        public string? PaymentMethod { get; set; }
        [StringLength(100, ErrorMessage = "Transaction reference cannot exceed 100 characters.")]
        public string? TransactionReference { get; set; }
        [StringLength(100, ErrorMessage = "Transaction reference cannot exceed 100 characters.")]
        public string? Status { get; set; }
        [StringLength(10, ErrorMessage = "Currency cannot exceed 10 characters.")]
        public string? Currency { get; set; }
        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters.")]
        public string? Notes { get; set; }
    }
}
