using Microsoft.Data.SqlClient;
using VentasAnalytics.Data.Interfaces;
using VentasAnalytics.Data.Models.Db;

namespace VentasAnalytics.Data.Repositories.Db;

public sealed class SalesRepository : ISalesRepository
{
    private readonly string _connectionString;

    public SalesRepository(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new ArgumentException(
                "La cadena de conexión no puede estar vacía.",
                nameof(connectionString));
        }

        _connectionString = connectionString;
    }

    public async Task<IReadOnlyCollection<Sale>> GetSalesAsync(
        CancellationToken cancellationToken = default)
    {
        const string query = """
            SELECT
                o.OrderID,
                od.DetailID,
                o.OrderDate,
                c.CustomerID,
                c.FirstName,
                c.LastName,
                c.Email,
                c.Phone,
                p.ProductID,
                p.ProductName,
                cat.CategoryID,
                cat.CategoryName,
                ci.CityID,
                ci.CityName,
                co.CountryID,
                co.CountryName,
                os.StatusID,
                os.StatusName,
                od.Quantity,
                od.UnitPrice,
                od.TotalPrice
            FROM Orders AS o
            INNER JOIN Order_Details AS od
                ON o.OrderID = od.OrderID
            INNER JOIN Customers AS c
                ON o.CustomerID = c.CustomerID
            INNER JOIN Products AS p
                ON od.ProductID = p.ProductID
            INNER JOIN Categories AS cat
                ON p.CategoryID = cat.CategoryID
            INNER JOIN Cities AS ci
                ON c.CityID = ci.CityID
            INNER JOIN Countries AS co
                ON ci.CountryID = co.CountryID
            INNER JOIN OrderStatus AS os
                ON o.StatusID = os.StatusID;
            """;

        var sales = new List<Sale>();

        await using var connection =
            new SqlConnection(_connectionString);

        await connection.OpenAsync(cancellationToken);

        await using var command =
            new SqlCommand(query, connection)
            {
                CommandTimeout = 120
            };

        await using var reader =
            await command.ExecuteReaderAsync(cancellationToken);

        // Se obtienen las posiciones una sola vez para mejorar el rendimiento.
        var orderIdOrdinal = reader.GetOrdinal("OrderID");
        var detailIdOrdinal = reader.GetOrdinal("DetailID");
        var orderDateOrdinal = reader.GetOrdinal("OrderDate");
        var customerIdOrdinal = reader.GetOrdinal("CustomerID");
        var firstNameOrdinal = reader.GetOrdinal("FirstName");
        var lastNameOrdinal = reader.GetOrdinal("LastName");
        var emailOrdinal = reader.GetOrdinal("Email");
        var phoneOrdinal = reader.GetOrdinal("Phone");
        var productIdOrdinal = reader.GetOrdinal("ProductID");
        var productNameOrdinal = reader.GetOrdinal("ProductName");
        var categoryIdOrdinal = reader.GetOrdinal("CategoryID");
        var categoryNameOrdinal = reader.GetOrdinal("CategoryName");
        var cityIdOrdinal = reader.GetOrdinal("CityID");
        var cityNameOrdinal = reader.GetOrdinal("CityName");
        var countryIdOrdinal = reader.GetOrdinal("CountryID");
        var countryNameOrdinal = reader.GetOrdinal("CountryName");
        var statusIdOrdinal = reader.GetOrdinal("StatusID");
        var statusNameOrdinal = reader.GetOrdinal("StatusName");
        var quantityOrdinal = reader.GetOrdinal("Quantity");
        var unitPriceOrdinal = reader.GetOrdinal("UnitPrice");
        var totalPriceOrdinal = reader.GetOrdinal("TotalPrice");

        while (await reader.ReadAsync(cancellationToken))
        {
            var sale = new Sale
            {
                OrderId = reader.GetInt32(orderIdOrdinal),
                DetailId = reader.GetInt32(detailIdOrdinal),
                OrderDate = reader.GetDateTime(orderDateOrdinal),

                CustomerId = reader.GetInt32(customerIdOrdinal),
                FirstName = GetString(reader, firstNameOrdinal),
                LastName = GetString(reader, lastNameOrdinal),
                Email = GetString(reader, emailOrdinal),
                Phone = GetString(reader, phoneOrdinal),

                ProductId = reader.GetInt32(productIdOrdinal),
                ProductName = GetString(reader, productNameOrdinal),

                CategoryId = reader.GetInt32(categoryIdOrdinal),
                CategoryName = GetString(reader, categoryNameOrdinal),

                CityId = reader.GetInt32(cityIdOrdinal),
                CityName = GetString(reader, cityNameOrdinal),

                CountryId = reader.GetInt32(countryIdOrdinal),
                CountryName = GetString(reader, countryNameOrdinal),

                StatusId = reader.GetInt32(statusIdOrdinal),
                StatusName = GetString(reader, statusNameOrdinal),

                Quantity = reader.GetInt32(quantityOrdinal),
                UnitPrice = reader.GetDecimal(unitPriceOrdinal),
                TotalPrice = reader.GetDecimal(totalPriceOrdinal)
            };

            sales.Add(sale);
        }

        return sales;
    }

    private static string GetString(
        SqlDataReader reader,
        int ordinal)
    {
        return reader.IsDBNull(ordinal)
            ? string.Empty
            : reader.GetString(ordinal);
    }
}