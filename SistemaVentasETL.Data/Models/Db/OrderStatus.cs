#nullable disable
namespace SistemaVentasETL.Data.Models.Db;

public partial class OrderStatus
{
    public int StatusId { get; set; }

    public string StatusName { get; set; }
}