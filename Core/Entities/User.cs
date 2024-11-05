using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Entities
{
    [Table("AppUsers")]
    public class User : IdentityUser<Guid>
    {
        [Required]
        [MaxLength(100)]
        public required string FirstName { get; set; }
        [Required]
        [MaxLength(100)]
        public required string LastName { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }
        public Cart Cart { get; set; }
        public virtual ICollection<PaymentTransaction> PaymentTransactions { get; set; }
        public string GetFullName()
        {
            return this.FirstName + " " + this.LastName;
        }
    }
}
