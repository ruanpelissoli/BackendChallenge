using BackendChallenge.Application.Accounts;
using Microsoft.AspNetCore.Identity;

namespace BackendChallenge.Application.Delivery;

public class DeliverymanSeed(
    RoleManager<IdentityRole> roleManager,
    UserManager<Account> userManager,
    ApplicationDbContext applicationDbContext)
{
    private readonly RoleManager<IdentityRole> _roleManager = roleManager;
    private readonly UserManager<Account> _userManager = userManager;

    public async Task SeedAsync()
    {
        var delivererUser = new Account { UserName = "deliveryman", Email = "deliveryman@example.com" };

        if (await _userManager.FindByNameAsync(delivererUser.UserName) == null)
        {
            await _userManager.CreateAsync(delivererUser, "Pass123!");
            await _userManager.AddToRoleAsync(delivererUser, Roles.Deliveryman);

            await applicationDbContext.Deliveryman.AddAsync(Deliveryman.Create(
                delivererUser.Id,
                "Delivery man",
                "76382843000161",
                new DateTime(1990, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                "491395413",
                CnhType.A
            ));
            await applicationDbContext.SaveChangesAsync();
        }
    }
}