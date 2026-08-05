using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaVentasETL.Data.Models.Db;

namespace SistemaVentasETL.Data.Context.Configurations
{
    public partial class CustomerConfiguration
        : IEntityTypeConfiguration<Customer>
    {
        public void Configure(EntityTypeBuilder<Customer> entity)
        {
            entity.Property(e => e.CustomerId)
                .ValueGeneratedOnAdd()
                .HasColumnName("CustomerID");

            entity.Property(e => e.CityId)
                .HasColumnName("CityID");

            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(150)
                .IsUnicode(false);

            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(100)
                .IsUnicode(false);

            entity.Property(e => e.Phone)
                .HasMaxLength(50)
                .IsUnicode(false);

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(
            EntityTypeBuilder<Customer> entity);
    }
}


