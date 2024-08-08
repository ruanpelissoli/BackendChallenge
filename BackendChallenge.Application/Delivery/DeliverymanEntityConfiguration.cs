using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BackendChallenge.Application.Delivery;
public sealed class DeliverymanEntityConfiguration : IEntityTypeConfiguration<Deliveryman>
{
    public void Configure(EntityTypeBuilder<Deliveryman> builder)
    {
        builder.ToTable("delivery_people");

        builder.HasKey(deliveryman => deliveryman.Id);

        builder.Property(deliveryMan => deliveryMan.Name)
            .HasColumnName("name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(deliveryMan => deliveryMan.Cnpj)
            .HasColumnName("cnpj")
            .HasMaxLength(14)
            .IsRequired();

        builder.Property(deliveryMan => deliveryMan.Birthdate)
            .HasColumnName("birthdate")
            .IsRequired();

        builder.Property(deliveryMan => deliveryMan.CnhNumber)
            .HasColumnName("cnh_number")
            .HasMaxLength(9)
            .IsRequired();

        builder.Property(deliveryMan => deliveryMan.CnhType)
            .HasColumnName("cnh_type")
            .HasConversion<string>()
            .IsRequired();

        builder.Property(deliveryMan => deliveryMan.CnhImageUrl)
            .HasColumnName("cnh_image_url")
            .HasMaxLength(500);

        builder.HasOne(r => r.Account)
            .WithOne()
            .HasForeignKey<Deliveryman>(r => r.AccountId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(deliveryMan => deliveryMan.Cnpj)
            .IsUnique();

        builder.HasIndex(deliveryMan => deliveryMan.CnhNumber)
            .IsUnique();
    }
}
