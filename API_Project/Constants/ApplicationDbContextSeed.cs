using API_Project.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API_Project.Constants
{
    public class ApplicationDbContextSeed
    {
        public static async Task SeedEssentialAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            // seed roles
            await roleManager.CreateAsync(new IdentityRole(Authorization.Roles.Admin.ToString()));
           // await roleManager.CreateAsync(new IdentityRole(Authorization.Roles.Moderator.ToString()));
            await roleManager.CreateAsync(new IdentityRole(Authorization.Roles.User.ToString()));

            var user = new ApplicationUser
            {
                UserName = Authorization.userName,
                Email = Authorization.email,
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };

            if (userManager.Users.All(u => u.Id != user.Id))
            {
                await userManager.CreateAsync(user, Authorization.password);
                await userManager.AddToRoleAsync(user, Authorization.role.ToString());
            }

        }
    }
}
