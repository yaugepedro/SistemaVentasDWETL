using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaVentasETL.Data.Models.Db;

namespace SistemaVentasETL.Data.Context.Configurations
{
    public partial class ProductConfiguration
        : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> entity)
        {
            entity.Property(e => e.ProductId)
                .ValueGeneratedOnAdd()
                .HasColumnName("ProductID");

            entity.Property(e => e.CategoryId)
                .HasColumnName("CategoryID");

            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)");

            entity.Property(e => e.ProductName)
                .IsRequired()
                .HasMaxLength(150)
                .IsUnicode(false);

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(
            EntityTypeBuilder<Product> entity);
    }
}

