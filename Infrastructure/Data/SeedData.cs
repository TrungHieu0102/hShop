using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.SeedWorks;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Data
{
    public class SeedData
    {
        public static async Task InitializeAsync(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            string roleName = Roles.Admin;
            var role = await roleManager.FindByNameAsync(roleName);
            if (role == null)
            {
                var roles = new Role { DisplayName = roleName, Id = new Guid(), Name = roleName };
                await roleManager.CreateAsync(roles);
            }

            string adminEmail = "admin@gmail.com";
            if (userManager.Users.All(u => u.UserName != adminEmail))
            {
                var adminUser = new User
                {
                    FirstName = "Admin",
                    LastName = "Admin",
                    UserName = adminEmail,
                    Email = adminEmail,
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@123$");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, roleName);
                }
            }
        }
    }
}