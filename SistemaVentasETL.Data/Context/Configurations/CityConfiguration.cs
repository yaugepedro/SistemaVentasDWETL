using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SistemaVentasETL.Data.Models.Db;

namespace SistemaVentasETL.Data.Context.Configurations
{
    public partial class CityConfiguration : IEntityTypeConfiguration<City>
    {
        public void Configure(EntityTypeBuilder<City> entity)
        {
            entity.Property(e => e.CityId)
    .ValueGeneratedOnAdd()
    .HasColumnName("CityID");
            entity.Property(e => e.CityName)
    .IsRequired()
    .HasMaxLength(100)
    .IsUnicode(false);
            entity.Property(e => e.CountryId).HasColumnName("CountryID");

            OnConfigurePartial(entity);
        }

        partial void OnConfigurePartial(EntityTypeBuilder<City> entity);
    }
}


