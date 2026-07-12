using MicroDemo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MicroDemo.Infrastructure.Persistence.Configurations;

public class UtenteConfiguration : IEntityTypeConfiguration<Utente>
{
    public void Configure(EntityTypeBuilder<Utente> builder)
    {
        builder.ToTable("Utenti");
        builder.HasKey(u => u.Id);

        builder.Property(u => u.FullName).IsRequired().HasMaxLength(150);
        builder.Property(u => u.Email).IsRequired().HasMaxLength(256);
        builder.Property(u => u.PhoneNumber).HasMaxLength(30);

        builder.HasIndex(u => u.Email).IsUnique();

        builder.HasOne(u => u.DefaultPrice)
            .WithMany()
            .HasForeignKey(u => u.DefaultPriceId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
