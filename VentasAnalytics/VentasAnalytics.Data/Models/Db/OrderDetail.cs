#nullable disable
namespace VentasAnalytics.Data.Models.Db;

public partial class OrderDetail
{
    public int DetailId { get; set; }

    public int OrderId { get; set; }

    public int ProductId { get; set; }

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal TotalPrice { get; set; }
}