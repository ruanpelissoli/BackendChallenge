using Microsoft.AspNetCore.Identity;

namespace BackendChallenge.Application.Accounts;
public class Seed(
    RoleManager<IdentityRole> roleManager,
    UserManager<Account> userManager)
{
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;
    private readonly UserManager<Account> _userManager = userManager;

    public async Task SeedAsync()
    {
        var roles = new[] { Roles.Admin, Roles.Deliveryman };

        foreach (var role in roles)
        {
            if (!await _roleManager.RoleExistsAsync(role))
            {
                await _roleManager.CreateAsync(new IdentityRole(role));
            }
        }

        var adminUser = new Account { UserName = "admin", Email = "admin@example.com" };
        var delivererUser = new Account { UserName = "deliveryman", Email = "deliveryman@example.com" };

        if (await _userManager.FindByNameAsync(adminUser.UserName) == null)
        {
            await _userManager.CreateAsync(adminUser, "Pass123!");
            await _userManager.AddToRoleAsync(adminUser, Roles.Admin);
        }

        if (await _userManager.FindByNameAsync(delivererUser.UserName) == null)
        {
            await _userManager.CreateAsync(delivererUser, "Pass123!");
            await _userManager.AddToRoleAsync(delivererUser, Roles.Deliveryman);
        }
    }
}
