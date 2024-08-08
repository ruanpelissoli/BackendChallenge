using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackendChallenge.Application.Rentals;

public class PlanConfiguration : IEntityTypeConfiguration<Plan>
{
    public void Configure(EntityTypeBuilder<Plan> builder)
    {
        builder.ToTable("plans");
        builder.HasKey(p => p.Id);

        builder.Property(p => p.DurationInDays)
            .IsRequired();

        builder.Property(p => p.CostPerDay)
            .IsRequired()
            .HasColumnType("decimal(18,2)");

        builder.Ignore(p => p.TotalValue);
    }
}
