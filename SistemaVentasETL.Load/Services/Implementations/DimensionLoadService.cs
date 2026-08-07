using SistemaVentasETL.Data.Services.Interfaces;
using System.Globalization;
using Microsoft.EntityFrameworkCore;
using SistemaVentasETL.Data.Context;
using SistemaVentasETL.Data.Models.Dimensions;
using SistemaVentasETL.Data.Models.Staging;
using SistemaVentasETL.Load.Services.Interfaces;
using SistemaVentasETL.Load.Utilities;

namespace SistemaVentasETL.Load.Services.Implementations;

public sealed class DimensionLoadService : IDimensionLoadService
{
    private readonly IJsonSourceService _jsonSourceService;
    private readonly IDimensionLookupService _dimensionLookupService;
    private readonly IDbContextFactory<WarehouseDbContext> _contextFactory;
    private readonly ILogger<DimensionLoadService> _logger;

    public DimensionLoadService(
        IJsonSourceService jsonSourceService,
        IDimensionLookupService dimensionLookupService,
        IDbContextFactory<WarehouseDbContext> contextFactory,
        ILogger<DimensionLoadService> logger)
    {
        _jsonSourceService = jsonSourceService;
        _dimensionLookupService = dimensionLookupService;
        _contextFactory = contextFactory;
        _logger = logger;
    }

    public async Task<ServiceResult<int>> LoadDimensionsAsync(
        CancellationToken cancellationToken = default)
    {
        try
        {
            var salesResult =
                await _jsonSourceService.GetSalesAsync(
                    cancellationToken);

            if (!salesResult.IsSuccess)
            {
                return ServiceResult<int>.Failure(
                    salesResult.Message);
            }

            var productsResult =
                await _jsonSourceService.GetProductsAsync(
                    cancellationToken);

            if (!productsResult.IsSuccess)
            {
                return ServiceResult<int>.Failure(
                    productsResult.Message);
            }

            var customersResult =
                await _jsonSourceService.GetCustomersAsync(
                    cancellationToken);

            if (!customersResult.IsSuccess)
            {
                return ServiceResult<int>.Failure(
                    customersResult.Message);
            }

            var sales =
                salesResult.Data ??
                Array.Empty<SaleStaging>();

            var products =
                productsResult.Data ??
                Array.Empty<ProductStaging>();

            var customers =
                customersResult.Data ??
                Array.Empty<CustomerStaging>();

            await using var context =
                await _contextFactory.CreateDbContextAsync(
                    cancellationToken);

            await using var transaction =
                await context.Database.BeginTransactionAsync(
                    cancellationToken);

            var insertedRecords = 0;

            insertedRecords += await LoadDatesAsync(
                context,
                sales,
                cancellationToken);

            insertedRecords += await LoadLocationsAsync(
                context,
                sales,
                customers,
                cancellationToken);

            insertedRecords += await LoadStatusesAsync(
                context,
                sales,
                cancellationToken);

            insertedRecords += await LoadSourcesAsync(
                context,
                cancellationToken);

            insertedRecords += await LoadCustomersAsync(
                context,
                customers,
                cancellationToken);

            insertedRecords += await LoadProductsAsync(
                context,
                products,
                cancellationToken);

            await context.SaveChangesAsync(
                cancellationToken);

            await transaction.CommitAsync(
                cancellationToken);

            _logger.LogInformation(
                "Se cargaron {RecordCount} registros en las dimensiones.",
                insertedRecords);

            return ServiceResult<int>.Success(
                insertedRecords,
                $"Se cargaron {insertedRecords} registros.");
        }
        catch (OperationCanceledException)
            when (cancellationToken.IsCancellationRequested)
        {
            _logger.LogWarning(
                "La carga de dimensiones fue cancelada.");

            return ServiceResult<int>.Failure(
                "La carga de dimensiones fue cancelada.");
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Ocurrió un error cargando las dimensiones.");

            return ServiceResult<int>.Failure(
                exception.Message);
        }
    }

    private static async Task<int> LoadDatesAsync(
        WarehouseDbContext context,
        IReadOnlyCollection<SaleStaging> sales,
        CancellationToken cancellationToken)
    {
        var savedDateKeys =
            await context.Fechas
                .AsNoTracking()
                .Select(date => date.IdFechaKey)
                .ToListAsync(cancellationToken);

        var existingKeys =
            savedDateKeys.ToHashSet();

        var culture =
            CultureInfo.GetCultureInfo("es-DO");

        var dates = sales
            .Select(sale => sale.OrderDate.Date)
            .Distinct()
            .Where(date =>
                !existingKeys.Contains(
                    CreateDateKey(date)));

        var inserted = 0;

        foreach (var date in dates)
        {
            var dateKey =
                CreateDateKey(date);

            context.Fechas.Add(
                new DimFecha
                {
                    IdFechaKey = dateKey,
                    Fecha = date,
                    Dia = date.Day,
                    Mes = date.Month,
                    NombreMes =
                        culture.DateTimeFormat.GetMonthName(
                            date.Month),
                    Trimestre =
                        ((date.Month - 1) / 3) + 1,
                    Anio = date.Year,
                    NombreDia =
                        culture.DateTimeFormat.GetDayName(
                            date.DayOfWeek),
                    EsFinDeSemana =
                        date.DayOfWeek is DayOfWeek.Saturday
                        or DayOfWeek.Sunday
                });

            existingKeys.Add(dateKey);
            inserted++;
        }

        return inserted;
    }

    private static async Task<int> LoadLocationsAsync(
        WarehouseDbContext context,
        IReadOnlyCollection<SaleStaging> sales,
        IReadOnlyCollection<CustomerStaging> customers,
        CancellationToken cancellationToken)
    {
        var savedLocations =
            await context.Ubicaciones
                .AsNoTracking()
                .ToListAsync(cancellationToken);

        var existingKeys = savedLocations
            .Select(location =>
                CreateLocationKey(
                    location.Ciudad,
                    location.Pais))
            .ToHashSet(
                StringComparer.OrdinalIgnoreCase);

        var locationsFromSales =
            sales.Select(sale => new
            {
                City = Clean(sale.CityName),
                Country = Clean(sale.CountryName)
            });

        var locationsFromCustomers =
            customers.Select(customer => new
            {
                City = Clean(customer.City),
                Country = Clean(customer.Country)
            });

        var locations = locationsFromSales
            .Concat(locationsFromCustomers)
            .Where(location =>
                location.City.Length > 0 ||
                location.Country.Length > 0)
            .GroupBy(
                location =>
                    CreateLocationKey(
                        location.City,
                        location.Country),
                StringComparer.OrdinalIgnoreCase)
            .Select(group =>
                group.First());

        var inserted = 0;

        foreach (var location in locations)
        {
            var key =
                CreateLocationKey(
                    location.City,
                    location.Country);

            if (!existingKeys.Add(key))
            {
                continue;
            }

            context.Ubicaciones.Add(
                new DimUbicacion
                {
                    Ciudad =
                        Limit(location.City, 100),
                    Region = null,
                    Pais =
                        Limit(location.Country, 100)
                });

            inserted++;
        }

        return inserted;
    }

    private static async Task<int> LoadStatusesAsync(
        WarehouseDbContext context,
        IReadOnlyCollection<SaleStaging> sales,
        CancellationToken cancellationToken)
    {
        var savedStatuses =
            await context.EstadosOrden
                .AsNoTracking()
                .Select(status =>
                    status.Estado)
                .ToListAsync(cancellationToken);

        var existingStatuses =
            savedStatuses.ToHashSet(
                StringComparer.OrdinalIgnoreCase);

        var statuses = sales
            .Select(sale =>
                Clean(sale.StatusName))
            .Where(status =>
                status.Length > 0)
            .Distinct(
                StringComparer.OrdinalIgnoreCase);

        var inserted = 0;

        foreach (var status in statuses)
        {
            if (!existingStatuses.Add(status))
            {
                continue;
            }

            context.EstadosOrden.Add(
                new DimEstadoOrden
                {
                    Estado =
                        Limit(status, 100)
                });

            inserted++;
        }

        return inserted;
    }

    private static async Task<int> LoadSourcesAsync(
        WarehouseDbContext context,
        CancellationToken cancellationToken)
    {
        var savedSources =
            await context.FuentesDatos
                .AsNoTracking()
                .Select(source =>
                    source.NombreFuente)
                .ToListAsync(cancellationToken);

        var existingSources =
            savedSources.ToHashSet(
                StringComparer.OrdinalIgnoreCase);

        var sources = new[]
        {
            new DimFuenteDatos
            {
                NombreFuente =
                    "Sistema_de_Ventas",
                TipoFuente =
                    "Base de datos transaccional",
                Descripcion =
                    "Ventas provenientes de la base transaccional Sistema_de_Ventas."
            },
            new DimFuenteDatos
            {
                NombreFuente =
                    "ProductosAPI",
                TipoFuente =
                    "API REST",
                Descripcion =
                    "Productos extraídos desde la API."
            },
            new DimFuenteDatos
            {
                NombreFuente =
                    "ClientesCSV",
                TipoFuente =
                    "Archivo CSV",
                Descripcion =
                    "Clientes extraídos desde el archivo CSV."
            }
        };

        var inserted = 0;

        foreach (var source in sources)
        {
            if (!existingSources.Add(
                source.NombreFuente))
            {
                continue;
            }

            context.FuentesDatos.Add(source);
            inserted++;
        }

        return inserted;
    }

    private async Task<int> LoadCustomersAsync(
        WarehouseDbContext context,
        IReadOnlyCollection<CustomerStaging> customers,
        CancellationToken cancellationToken)
    {
        var currentById =
            await _dimensionLookupService
                .GetCurrentCustomersAsync(
                    cancellationToken);

        var currentDate =
            DateTime.Today;

        var inserted = 0;

        var uniqueCustomers = customers
            .GroupBy(customer =>
                customer.CustomerId)
            .Select(group =>
                group.First());

        foreach (var customer in uniqueCustomers)
        {
            var name = Limit(
                Clean(
                    $"{customer.FirstName} {customer.LastName}"),
                150);

            var city = Limit(
                Clean(customer.City),
                100);

            var country = Limit(
                Clean(customer.Country),
                100);

            if (currentById.TryGetValue(
                customer.CustomerId,
                out var currentCustomer))
            {
                var changed =
                    !Same(
                        currentCustomer.Nombre,
                        name) ||
                    !Same(
                        currentCustomer.Ciudad,
                        city) ||
                    !Same(
                        currentCustomer.Pais,
                        country);

                if (!changed)
                {
                    continue;
                }

                var customerToClose =
                    new DimCliente
                    {
                        IdClienteKey =
                            currentCustomer.IdClienteKey,
                        EsActual = false,
                        FechaFin = currentDate
                    };

                var customerEntry =
                    context.Clientes.Attach(
                        customerToClose);

                customerEntry
                    .Property(item =>
                        item.EsActual)
                    .IsModified = true;

                customerEntry
                    .Property(item =>
                        item.FechaFin)
                    .IsModified = true;
            }

            context.Clientes.Add(
                new DimCliente
                {
                    IdClienteOrigen =
                        customer.CustomerId,
                    Nombre = name,
                    TipoCliente =
                        "Cliente",
                    Ciudad = city,
                    Region = null,
                    Pais = country,
                    FechaInicio =
                        currentDate,
                    FechaFin = null,
                    EsActual = true
                });

            inserted++;
        }

        return inserted;
    }

    private async Task<int> LoadProductsAsync(
        WarehouseDbContext context,
        IReadOnlyCollection<ProductStaging> products,
        CancellationToken cancellationToken)
    {
        var currentById =
            await _dimensionLookupService
                .GetCurrentProductsAsync(
                    cancellationToken);

        var currentDate =
            DateTime.Today;

        var inserted = 0;

        var uniqueProducts = products
            .GroupBy(product =>
                product.ProductId)
            .Select(group =>
                group.First());

        foreach (var product in uniqueProducts)
        {
            var name = Limit(
                Clean(product.ProductName),
                150);

            var category = Limit(
                Clean(product.CategoryName),
                100);

            if (currentById.TryGetValue(
                product.ProductId,
                out var currentProduct))
            {
                var changed =
                    !Same(
                        currentProduct.NombreProducto,
                        name) ||
                    !Same(
                        currentProduct.Categoria,
                        category) ||
                    currentProduct.Precio !=
                        product.Price;

                if (!changed)
                {
                    continue;
                }

                var productToClose =
                    new DimProducto
                    {
                        IdProductoKey =
                            currentProduct.IdProductoKey,
                        EsActual = false,
                        FechaFin = currentDate
                    };

                var productEntry =
                    context.Productos.Attach(
                        productToClose);

                productEntry
                    .Property(item =>
                        item.EsActual)
                    .IsModified = true;

                productEntry
                    .Property(item =>
                        item.FechaFin)
                    .IsModified = true;
            }

            context.Productos.Add(
                new DimProducto
                {
                    IdProductoOrigen =
                        product.ProductId,
                    NombreProducto = name,
                    Categoria = category,
                    Precio = product.Price,
                    FechaInicio =
                        currentDate,
                    FechaFin = null,
                    EsActual = true
                });

            inserted++;
        }

        return inserted;
    }

    private static int CreateDateKey(
        DateTime date)
    {
        return (date.Year * 10000) +
               (date.Month * 100) +
               date.Day;
    }

    private static string CreateLocationKey(
        string? city,
        string? country)
    {
        return $"{Clean(city)}|{Clean(country)}";
    }

    private static string Clean(
        string? value)
    {
        return value?.Trim() ??
               string.Empty;
    }

    private static string Limit(
        string value,
        int maximumLength)
    {
        return value.Length <= maximumLength
            ? value
            : value[..maximumLength];
    }

    private static bool Same(
        string? first,
        string? second)
    {
        return string.Equals(
            Clean(first),
            Clean(second),
            StringComparison.OrdinalIgnoreCase);
    }
}