using MicroDemo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MicroDemo.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");
        builder.HasKey(o => o.Id);

        builder.Property(o => o.TotalAmount).HasColumnType("decimal(18,2)");
        builder.Property(o => o.Currency).IsRequired().HasMaxLength(3).IsFixedLength();
        builder.Property(o => o.Status).HasConversion<int>();

        builder.HasOne(o => o.Utente)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.UtenteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(o => o.Subscription)
            .WithMany()
            .HasForeignKey(o => o.SubscriptionId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
