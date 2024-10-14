using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DTOs
{
    public class PermissionDto
    {
        public string RoleId { get; set; } = string.Empty;
        public IList<RoleClaimsDto> RoleClaims { get; set; } = new List<RoleClaimsDto>();
    }
}