using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackendChallenge.Application.Rentals;

public class RentalConfiguration : IEntityTypeConfiguration<Rental>
{
    public void Configure(EntityTypeBuilder<Rental> builder)
    {
        builder.ToTable("rentals");
        builder.HasKey(r => r.Id);

        builder.Property(r => r.StartDate)
            .IsRequired();

        builder.Property(r => r.EndDate)
            .IsRequired();

        builder.HasOne(r => r.Bike)
           .WithMany()
           .HasForeignKey(r => r.BikeId)
           .IsRequired()
           .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Deliveryman)
            .WithMany()
            .HasForeignKey(r => r.DeliverymanId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(r => r.Plan)
            .WithMany()
            .HasForeignKey(r => r.PlanId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);
    }
}