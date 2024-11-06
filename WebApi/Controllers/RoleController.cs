using AutoMapper;
using Core.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApi.Filters;
using Core.Model;
using Microsoft.EntityFrameworkCore;
using Core.SeedWorks.Constants;
using System.Reflection;
using Application.Extensions;
using Application.DTOs.RoleDto;
using Application.DTOs.AuthsDto;
using Application.DTOs.RolesDto;
using Application.DTOs.RoleDto.RolesDto;

#nullable disable
namespace WebApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    [ValidateModel]
    public class RoleController(RoleManager<Role> roleManager, IMapper mapper) : ControllerBase
    {
        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> CreateRole([FromBody] CreateUpdateRoleRequest request)
        {
            var result = await roleManager.CreateAsync(new Role
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                DisplayName = request.DisplayName
            });
            return Ok(result);
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteRoles([FromQuery] Guid[] ids)
        {
            foreach (var id in ids)
            {
                var role = await roleManager.FindByIdAsync(id.ToString());
                if (role == null)
                {
                    return NotFound();
                }

                await roleManager.DeleteAsync(role);
            }

            return Ok();
        }

        [HttpGet("{roleId:guid}")]
        public async Task<IActionResult> GetRoleById([FromRoute] Guid roleId)
        {
            var role = await roleManager.FindByIdAsync(roleId.ToString());
            if (role == null)
            {
                return NotFound();
            }

            return Ok(mapper.Map<RoleDto>(role));
        }

        [HttpGet]
        [Route("all/paging")]
        public async Task<ActionResult<PagedResult<RoleDto>>> GetRolesAllPaging(string keyword, int pageIndex = 1,
            int pageSize = 10, bool IsDecsending = false)
        {
            var query = roleManager.Roles;
            if (!string.IsNullOrEmpty(keyword))
                query = query.Where(x => x.Name.Contains(keyword)
                                         || x.DisplayName.Contains(keyword));
            query = IsDecsending ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name);
            var totalRow = query.Count();
            query = query.Skip((pageIndex - 1) * pageSize)
                .Take(pageSize);

            var data = await mapper.ProjectTo<RoleDto>(query).ToListAsync();
            var paginationSet = new PagedResult<RoleDto>
            {
                Results = data,
                CurrentPage = pageIndex,
                RowCount = totalRow,
                PageSize = pageSize
            };

            return Ok(paginationSet);
        }

        [HttpGet]
        [Route("all")]
        public async Task<ActionResult<List<RoleDto>>> GetAllRoles()
        {
            var model = await mapper.ProjectTo<RoleDto>(roleManager.Roles).ToListAsync();
            return Ok(model);
        }

        [HttpPut("permissions")]
        public async Task<IActionResult> SavePermission([FromBody] PermissionDto model)
        {
            var role = await roleManager.FindByIdAsync(model.RoleId);
            if (role == null)
                return NotFound();

            var claims = await roleManager.GetClaimsAsync(role);
            foreach (var claim in claims)
            {
                await roleManager.RemoveClaimAsync(role, claim);
            }

            var selectedClaims = model.RoleClaims.Where(a => a.Selected).ToList();
            foreach (var claim in selectedClaims)
            {
                await roleManager.AddPermissionClaim(role, claim.Value);
            }

            return Ok();
        }

        [HttpGet("{roleId:guid}/permissions")]
        public async Task<ActionResult<PermissionDto>> GetAllRolePermissions([FromRoute]Guid roleId)
        {
            var model = new PermissionDto();
            var allPermissions = new List<RoleClaimsDto>();
            var types = typeof(Permissions).GetTypeInfo().DeclaredNestedTypes;
            foreach (var type in types)
            {
                allPermissions.GetPermissions(type);
            }

            var role = await roleManager.FindByIdAsync(roleId.ToString());
            if (role == null)
                return NotFound();
            model.RoleId = roleId.ToString();
            var claims = await roleManager.GetClaimsAsync(role);
            var allClaimValues = allPermissions.Select(a => a.Value).ToList();
            var roleClaimValues = claims.Select(a => a.Value).ToList();
            var authorizedClaims = allClaimValues.Intersect(roleClaimValues).ToList();
            foreach (var permission in allPermissions)
            {
                if (authorizedClaims.Any(a => a == permission.Value))
                {
                    permission.Selected = true;
                }
            }

            model.RoleClaims = allPermissions;
            return Ok(model);
        }
    }
}