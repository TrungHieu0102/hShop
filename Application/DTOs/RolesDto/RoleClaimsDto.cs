using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Application.DTOs.RolesDto
{
    public class RoleClaimsDto
    {
        public required string Type { get; set; }
        public required string Value { get; set; }
        public string? DisplayName { get; set; }
        public bool Selected { get; set; }
    }
}