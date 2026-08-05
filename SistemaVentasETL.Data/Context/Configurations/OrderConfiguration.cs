using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaVentasETL.Data.Models.Db;

namespace SistemaVentasETL.Data.Context.Configurations
{
    public partial class OrderConfiguration
        : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> entity)
        {
            entity.Property(e => e.OrderId)
                .ValueGeneratedOnAdd()
                .HasColumnName("OrderID");

            entity.Property(e => e.CustomerId)
                .HasColumnName("CustomerID");

            entity.Property(e => e.OrderDate)
                .HasColumnType("date");

            entity.Property(e => e.StatusId)
                .HasColumnName("StatusID");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(
            EntityTypeBuilder<Order> entity);
    }
}

