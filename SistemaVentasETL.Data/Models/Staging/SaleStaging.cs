namespace SistemaVentasETL.Data.Models.Staging;

public sealed class SaleStaging
{
    public int OrderId { get; set; }

    public int DetailId { get; set; }

    public DateTime OrderDate { get; set; }

    public int CustomerId { get; set; }

    public string FirstName { get; set; } = string.Empty;

    public string LastName { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public int ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public int CategoryId { get; set; }

    public string CategoryName { get; set; } = string.Empty;

    public int CityId { get; set; }

    public string CityName { get; set; } = string.Empty;

    public int CountryId { get; set; }

    public string CountryName { get; set; } = string.Empty;

    public int StatusId { get; set; }

    public string StatusName { get; set; } = string.Empty;

    public int Quantity { get; set; }

    public decimal UnitPrice { get; set; }

    public decimal TotalPrice { get; set; }
}