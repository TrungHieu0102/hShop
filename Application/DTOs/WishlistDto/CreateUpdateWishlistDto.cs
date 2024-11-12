using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.WishlistDto
{
    public class CreateUpdateWishlistDto
    {
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
    }
}
