using System.Threading.Tasks;
using Mebeller.Data;
using Mebeller.Data.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace Mebeller.Config;

public static class SeedUsers
{
    public const string AdminRoleName = nameof(Roles.Admin);
    public const string UserRoleName = nameof(Roles.User);

    public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        // Seed Roles
        if (!await roleManager.RoleExistsAsync(AdminRoleName))
        {
            await roleManager.CreateAsync(new IdentityRole(AdminRoleName));
        }

        if (!await roleManager.RoleExistsAsync(UserRoleName))
        {
            await roleManager.CreateAsync(new IdentityRole(UserRoleName));
        }
    }

    public static async Task SeedAdminAsync(UserManager<ApplicationUser> userManager, IConfiguration configuration)
    {
        // Seed Default User
        var defaultUser = new ApplicationUser
        {
            UserName = configuration["Admin:UserName"],
            Email = configuration["Admin:Email"],
            EmailConfirmed = true,
            PhoneNumberConfirmed = true
        };

        var userExists = await userManager.FindByNameAsync(defaultUser.UserName) != null;

        if (!userExists)
        {
            var defaultPassword = configuration["Admin:Password"];
            var result = await userManager.CreateAsync(defaultUser, defaultPassword);

            if (result.Succeeded)
                // Add the user to the roles
                await userManager.AddToRoleAsync(defaultUser, AdminRoleName);
        }
    }

    private enum Roles
    {
        Admin,
        User
    }
}