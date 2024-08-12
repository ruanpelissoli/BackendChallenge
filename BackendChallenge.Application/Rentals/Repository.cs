using BackendChallenge.Application.Bikes;
using BackendChallenge.Application.Delivery;
using Microsoft.EntityFrameworkCore;

namespace BackendChallenge.Application.Rentals;

public interface IRepository
{
    Task CreateRental(Rental rental);
    Task<Rental?> GetRentalById(Guid id);
    Task<Bike?> GetBikeById(Guid id);
    Task<Plan?> GetPlanById(Guid id);
    Task<Deliveryman?> GetDelivymanByAccountId(string accountId);
    Task<bool> IsBikeAvailableForRental(Guid bikeId, DateOnly startDate, DateOnly endDate);
    Task Commit();
}

public class Repository(ApplicationDbContext context) : IRepository
{
    public async Task<Rental?> GetRentalById(Guid id)
    {
        return await context.Rentals
            .Include(r => r.Plan)
            .FirstOrDefaultAsync(r => r.Id == id);
    }

    public async Task<Deliveryman?> GetDelivymanByAccountId(string accountId)
    {
        return await context.Deliveryman.FirstOrDefaultAsync(d => d.AccountId == accountId);
    }

    public async Task CreateRental(Rental rental)
    {
        await context.Rentals.AddAsync(rental);
    }

    public async Task<Bike?> GetBikeById(Guid id)
    {
        return await context.Bikes.FirstOrDefaultAsync(b => b.Id == id);
    }

    public Task<Plan?> GetPlanById(Guid id)
    {
        return context.Plans.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task Commit()
    {
        await context.SaveChangesAsync();
    }

    public async Task<bool> IsBikeAvailableForRental(Guid bikeId, DateOnly startDate, DateOnly endDate)
    {
        var rentals = await context.Rentals
            .Where(r => r.BikeId == bikeId)
            .Where(r => r.StartDate <= startDate && r.EndDate >= endDate)
            .ToListAsync();

        return rentals.Count == 0;
    }
}
