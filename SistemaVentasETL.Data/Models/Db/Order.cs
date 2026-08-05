#nullable disable
namespace SistemaVentasETL.Data.Models.Db;

public partial class Order
{
    public int OrderId { get; set; }

    public int CustomerId { get; set; }

    public int StatusId { get; set; }

    public DateTime OrderDate { get; set; }
}