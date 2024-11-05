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
    public class TransactionDto
    {
        public Guid TransactionId { get; set; }
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; }
        public string TransactionReference { get; set; }
        public string Status { get; set; }
        public string Currency { get; set; }
        public string? Notes { get; set; }
    }
}
