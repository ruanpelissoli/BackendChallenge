using BackendChallenge.Application.Accounts;
using BackendChallenge.CrossCutting.Services;

namespace BackendChallenge.Application.Delivery;

public class DeliverymanSeed(
    IAccountService<Account> accountService,
    ApplicationDbContext applicationDbContext)
{

    public async Task SeedAsync()
    {
        var delivererUser = new Account { UserName = "deliveryman", Email = "deliveryman@example.com" };

        if (await accountService.FindByNameAsync(delivererUser.UserName) == null)
        {
            await accountService.CreateAccount(delivererUser, "Pass123!", Roles.Deliveryman);

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