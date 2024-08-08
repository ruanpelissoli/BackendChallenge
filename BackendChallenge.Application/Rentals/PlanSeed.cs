namespace BackendChallenge.Application.Rentals;

public class PlanSeed(ApplicationDbContext context)
{
    public async Task SeedAsync()
    {
        if (context.Plans.Any())
            return;

        List<Plan> plans =
        [
            new(7, 30.00M, 20.00M),
            new(15, 28.00M, 40.00M),
            new(30, 22.00M),
            new(45, 20.00M),
            new(50, 18.00M),
        ];

        await context.Plans.AddRangeAsync(plans);
        await context.SaveChangesAsync();
    }
}