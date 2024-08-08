using BackendChallenge.Application.Bikes;
using BackendChallenge.Application.Delivery;
using Microsoft.EntityFrameworkCore;

namespace BackendChallenge.Application;
public sealed class ApplicationDbContext : DbContext
{
    public DbSet<Bike> Bikes { get; init; }
    public DbSet<Deliveryman> Deliveryman { get; init; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
