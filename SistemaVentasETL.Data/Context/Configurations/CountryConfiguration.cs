using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaVentasETL.Data.Models.Db;

namespace SistemaVentasETL.Data.Context.Configurations
{
    public partial class CountryConfiguration : IEntityTypeConfiguration<Country>
    {
        public void Configure(EntityTypeBuilder<Country> entity)
        {
           entity.Property(e => e.CountryId)
    .ValueGeneratedOnAdd()
    .HasColumnName("CountryID");
            entity.Property(e => e.CountryName)
    .IsRequired()
    .HasMaxLength(100)
    .IsUnicode(false);

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<Country> entity);
    }
}


