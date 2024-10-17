


using Application.DTOs.RolesDto;

namespace Application.DTOs.AuthsDto
{
    public class PermissionDto
    {
        public string RoleId { get; set; } = string.Empty;
        public IList<RoleClaimsDto> RoleClaims { get; set; } = new List<RoleClaimsDto>();
    }
}