using MicroDemo.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace MicroDemo.Infrastructure.Persistence.Configurations;

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        builder.ToTable("OrderItems");
        builder.HasKey(oi => oi.Id);

        builder.Property(oi => oi.UnitPrice).HasColumnType("decimal(18,2)");
        builder.Property(oi => oi.LineTotal).HasColumnType("decimal(18,2)");
        builder.Property(oi => oi.Currency).IsRequired().HasMaxLength(3).IsFixedLength();

        builder.HasIndex(oi => oi.OrderId);
        builder.HasIndex(oi => oi.ProductId);

        // Order (Cascade) and Product (Restrict) relationships are already configured in
        // OrderConfiguration and ProductConfiguration respectively.
    }
}
