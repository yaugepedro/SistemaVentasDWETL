#nullable disable
namespace SistemaVentasETL.Data.Models.Db;

public partial class Customer
{
    public int CustomerId { get; set; }

    public string FirstName { get; set; }

    public string LastName { get; set; }

    public string Email { get; set; }

    public string Phone { get; set; }

    public int CityId { get; set; }
}