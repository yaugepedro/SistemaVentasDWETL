using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaVentasETL.Data.Models.Db;

namespace SistemaVentasETL.Data.Context.Configurations
{
    public partial class OrderStatusConfiguration
        : IEntityTypeConfiguration<OrderStatus>
    {
        public void Configure(EntityTypeBuilder<OrderStatus> entity)
        {
            entity.HasKey(e => e.StatusId);

            entity.ToTable("OrderStatus");

            entity.Property(e => e.StatusId)
                .ValueGeneratedOnAdd()
                .HasColumnName("StatusID");

            entity.Property(e => e.StatusName)
                .IsRequired()
                .HasMaxLength(50)
                .IsUnicode(false);

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(
            EntityTypeBuilder<OrderStatus> entity);
    }
}

