using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System.Data;
using Microsoft.Data.SqlClient;
using SistemaVentasETL.Data.Models.Lookup;
using SistemaVentasETL.Data.Services.Interfaces;

namespace SistemaVentasETL.Data.Services.Implementations;

public sealed class DimensionLookupService
    : IDimensionLookupService
{
    private readonly string _connectionString;
    private readonly ILogger<DimensionLookupService> _logger;

    public DimensionLookupService(
        IConfiguration configuration,
        ILogger<DimensionLookupService> logger)
    {
        _connectionString =
            configuration.GetConnectionString(
                "DW_Sistema_Ventas")
            ?? throw new InvalidOperationException(
                "No se encontró la conexión DW_Sistema_Ventas.");

        _logger = logger;
    }

    public async Task<
        IReadOnlyDictionary<int, CustomerDimensionLookup>>
        GetCurrentCustomersAsync(
            CancellationToken cancellationToken = default)
    {
        const string query = """
            SELECT
                IdClienteKey,
                IdClienteOrigen,
                Nombre,
                Ciudad,
                Pais
            FROM dim.DimCliente
            WHERE EsActual = 1;
            """;

        var customers =
            new Dictionary<int, CustomerDimensionLookup>();

        await using var connection =
            new SqlConnection(_connectionString);

        await connection.OpenAsync(
            cancellationToken);

        await using var command =
            new SqlCommand(query, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 120
            };

        await using var reader =
            await command.ExecuteReaderAsync(
                cancellationToken);

        var customerKeyOrdinal =
            reader.GetOrdinal("IdClienteKey");

        var customerOriginOrdinal =
            reader.GetOrdinal("IdClienteOrigen");

        var nameOrdinal =
            reader.GetOrdinal("Nombre");

        var cityOrdinal =
            reader.GetOrdinal("Ciudad");

        var countryOrdinal =
            reader.GetOrdinal("Pais");

        while (await reader.ReadAsync(
            cancellationToken))
        {
            var customerKey =
                reader.GetInt32(
                    customerKeyOrdinal);

            var customerOriginId =
                reader.GetInt32(
                    customerOriginOrdinal);

            var name =
                reader.GetString(
                    nameOrdinal);

            var city =
                GetNullableString(
                    reader,
                    cityOrdinal);

            var country =
                GetNullableString(
                    reader,
                    countryOrdinal);

            customers[customerOriginId] =
                new CustomerDimensionLookup(
                    customerKey,
                    customerOriginId,
                    name,
                    city,
                    country);
        }

        _logger.LogInformation(
            "ADO.NET leyó {RecordCount} clientes vigentes.",
            customers.Count);

        return customers;
    }

    public async Task<
        IReadOnlyDictionary<int, ProductDimensionLookup>>
        GetCurrentProductsAsync(
            CancellationToken cancellationToken = default)
    {
        const string query = """
            SELECT
                IdProductoKey,
                IdProductoOrigen,
                NombreProducto,
                Categoria,
                Precio
            FROM dim.DimProducto
            WHERE EsActual = 1;
            """;

        var products =
            new Dictionary<int, ProductDimensionLookup>();

        await using var connection =
            new SqlConnection(_connectionString);

        await connection.OpenAsync(
            cancellationToken);

        await using var command =
            new SqlCommand(query, connection)
            {
                CommandType = CommandType.Text,
                CommandTimeout = 120
            };

        await using var reader =
            await command.ExecuteReaderAsync(
                cancellationToken);

        var productKeyOrdinal =
            reader.GetOrdinal("IdProductoKey");

        var productOriginOrdinal =
            reader.GetOrdinal("IdProductoOrigen");

        var nameOrdinal =
            reader.GetOrdinal("NombreProducto");

        var categoryOrdinal =
            reader.GetOrdinal("Categoria");

        var priceOrdinal =
            reader.GetOrdinal("Precio");

        while (await reader.ReadAsync(
            cancellationToken))
        {
            var productKey =
                reader.GetInt32(
                    productKeyOrdinal);

            var productOriginId =
                reader.GetInt32(
                    productOriginOrdinal);

            var name =
                reader.GetString(
                    nameOrdinal);

            var category =
                GetNullableString(
                    reader,
                    categoryOrdinal);

            var price =
                GetNullableDecimal(
                    reader,
                    priceOrdinal);

            products[productOriginId] =
                new ProductDimensionLookup(
                    productKey,
                    productOriginId,
                    name,
                    category,
                    price);
        }

        _logger.LogInformation(
            "ADO.NET leyó {RecordCount} productos vigentes.",
            products.Count);

        return products;
    }

    private static string? GetNullableString(
        SqlDataReader reader,
        int ordinal)
    {
        return reader.IsDBNull(ordinal)
            ? null
            : reader.GetString(ordinal);
    }

    private static decimal? GetNullableDecimal(
        SqlDataReader reader,
        int ordinal)
    {
        return reader.IsDBNull(ordinal)
            ? null
            : reader.GetDecimal(ordinal);
    }
}