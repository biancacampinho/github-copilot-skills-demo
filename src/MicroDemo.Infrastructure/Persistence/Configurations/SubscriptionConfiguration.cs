using MicroDemo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MicroDemo.Infrastructure.Persistence.Configurations;

public class SubscriptionConfiguration : IEntityTypeConfiguration<Subscription>
{
    public void Configure(EntityTypeBuilder<Subscription> builder)
    {
        builder.ToTable("Subscriptions");
        builder.HasKey(s => s.Id);

        builder.Property(s => s.Status).HasConversion<int>();

        builder.HasOne(s => s.Utente)
            .WithMany(u => u.Subscriptions)
            .HasForeignKey(s => s.UtenteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(s => s.Price)
            .WithMany(p => p.Subscriptions)
            .HasForeignKey(s => s.PriceId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(s => new { s.UtenteId, s.Status });
    }
}
