using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackendChallenge.Application.Bikes;
internal sealed class BikeEntityConfiguration : IEntityTypeConfiguration<Bike>
{
    public void Configure(EntityTypeBuilder<Bike> builder)
    {
        builder.ToTable("bikes");

        builder.HasKey(bike => bike.Id);

        builder.Property(bike => bike.Year)
            .HasColumnName("year")
            .HasMaxLength(4)
            .IsRequired();

        builder.Property(bike => bike.Model)
            .HasColumnName("model")
            .HasMaxLength(50)
            .IsRequired();

        builder.Property(bike => bike.LicensePlate)
            .HasColumnName("license_plate")
            .HasMaxLength(7)
            .IsRequired();

        builder.HasIndex(bike => bike.LicensePlate)
            .IsUnique();
    }
}
