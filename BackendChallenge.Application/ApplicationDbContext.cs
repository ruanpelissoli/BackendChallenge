using BackendChallenge.Application.Accounts;
using BackendChallenge.Application.Bikes;
using BackendChallenge.Application.Delivery;
using BackendChallenge.Application.Rentals;
using Dapper;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace BackendChallenge.Application;
public sealed class ApplicationDbContext : DbContext
{
    public DbSet<Bike> Bikes { get; init; }
    public DbSet<Deliveryman> Deliveryman { get; init; }
    public DbSet<Plan> Plans { get; init; }
    public DbSet<Rental> Rentals { get; init; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>(entity =>
        {
            entity.ToTable("AspNetUsers");
        });

        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(ApplicationDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}

internal sealed class DateOnlyTypeHandler : SqlMapper.TypeHandler<DateOnly>
{
    public override DateOnly Parse(object value) => DateOnly.FromDateTime((DateTime)value);

    public override void SetValue(IDbDataParameter parameter, DateOnly value)
    {
        parameter.DbType = DbType.Date;
        parameter.Value = value;
    }
}