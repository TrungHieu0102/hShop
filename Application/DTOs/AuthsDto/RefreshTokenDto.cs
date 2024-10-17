using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.DTOs.AuthsDto
{
    public class RefreshTokenDto
    {
        public string Token { get; set; }
        public string UserId { get; set; }
    }
}
