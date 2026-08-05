using Microsoft.EntityFrameworkCore;
using VentasAnalytics.Data.Models.Db;
#nullable disable

namespace VentasAnalytics.Data.Context;

public partial class VentasDBContext : DbContext
{
    public VentasDBContext(DbContextOptions<VentasDBContext> options)
        : base(options)
    {
    }

    public DbSet<Category> Categories { get; set; }

    public DbSet<City> Cities { get; set; }

    public DbSet<Country> Countries { get; set; }

    public DbSet<Customer> Customers { get; set; }

    public DbSet<Order> Orders { get; set; }

    public DbSet<OrderDetail> OrderDetails { get; set; }

    public DbSet<OrderStatus> OrderStatuses { get; set; }

    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new Configurations.CategoryConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.CityConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.CountryConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.CustomerConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.OrderConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.OrderDetailConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.OrderStatusConfiguration());
        modelBuilder.ApplyConfiguration(new Configurations.ProductConfiguration());

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
