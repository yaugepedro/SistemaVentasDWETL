using Microsoft.EntityFrameworkCore;
using SistemaVentasETL.Data.Models.Dimensions;

namespace SistemaVentasETL.Data.Context;

public sealed class WarehouseDbContext : DbContext
{
    public WarehouseDbContext(
        DbContextOptions<WarehouseDbContext> options)
        : base(options)
    {
    }

    public DbSet<DimFecha> Fechas =>
        Set<DimFecha>();

    public DbSet<DimUbicacion> Ubicaciones =>
        Set<DimUbicacion>();

    public DbSet<DimEstadoOrden> EstadosOrden =>
        Set<DimEstadoOrden>();

    public DbSet<DimCliente> Clientes =>
        Set<DimCliente>();

    public DbSet<DimProducto> Productos =>
        Set<DimProducto>();

    public DbSet<DimFuenteDatos> FuentesDatos =>
        Set<DimFuenteDatos>();

    protected override void OnModelCreating(
        ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<DimFecha>(entity =>
        {
            entity.Property(e => e.IdFechaKey)
                .ValueGeneratedNever();

            entity.Property(e => e.Fecha)
                .HasColumnType("date");

            entity.Property(e => e.NombreMes)
                .HasMaxLength(20);

            entity.Property(e => e.NombreDia)
                .HasMaxLength(20);
        });

        modelBuilder.Entity<DimUbicacion>(entity =>
        {
            entity.Property(e => e.Ciudad)
                .HasMaxLength(100);

            entity.Property(e => e.Region)
                .HasMaxLength(100);

            entity.Property(e => e.Pais)
                .HasMaxLength(100);
        });

        modelBuilder.Entity<DimEstadoOrden>(entity =>
        {
            entity.Property(e => e.Estado)
                .HasMaxLength(100)
                .IsRequired();
        });

        modelBuilder.Entity<DimCliente>(entity =>
        {
            entity.Property(e => e.FechaInicio)
                .HasColumnType("date");

            entity.Property(e => e.FechaFin)
                .HasColumnType("date");
        });

        modelBuilder.Entity<DimProducto>(entity =>
        {
            entity.Property(e => e.Precio)
                .HasColumnType("decimal(18,2)");

            entity.Property(e => e.FechaInicio)
                .HasColumnType("date");

            entity.Property(e => e.FechaFin)
                .HasColumnType("date");
        });
    }
}