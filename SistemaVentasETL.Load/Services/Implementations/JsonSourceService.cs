using System.Text.Json;
using SistemaVentasETL.Data.Models.Staging;
using SistemaVentasETL.Load.Services.Interfaces;
using SistemaVentasETL.Load.Utilities;

namespace SistemaVentasETL.Load.Services.Implementations;

public sealed class JsonSourceService : IJsonSourceService
{
    private readonly IConfiguration _configuration;
    private readonly IHostEnvironment _environment;
    private readonly ILogger<JsonSourceService> _logger;

    private readonly JsonSerializerOptions _jsonOptions =
        new()
        {
            PropertyNameCaseInsensitive = true
        };

    public JsonSourceService(
        IConfiguration configuration,
        IHostEnvironment environment,
        ILogger<JsonSourceService> logger)
    {
        _configuration = configuration;
        _environment = environment;
        _logger = logger;
    }

    public Task<ServiceResult<IReadOnlyCollection<SaleStaging>>>
        GetSalesAsync(
            CancellationToken cancellationToken = default)
    {
        return ReadJsonAsync<SaleStaging>(
            "StagingFiles:SalesPath",
            cancellationToken);
    }

    public Task<ServiceResult<IReadOnlyCollection<ProductStaging>>>
        GetProductsAsync(
            CancellationToken cancellationToken = default)
    {
        return ReadJsonAsync<ProductStaging>(
            "StagingFiles:ProductsPath",
            cancellationToken);
    }

    public Task<ServiceResult<IReadOnlyCollection<CustomerStaging>>>
        GetCustomersAsync(
            CancellationToken cancellationToken = default)
    {
        return ReadJsonAsync<CustomerStaging>(
            "StagingFiles:CustomersPath",
            cancellationToken);
    }

    private async Task<ServiceResult<IReadOnlyCollection<T>>>
        ReadJsonAsync<T>(
            string configurationKey,
            CancellationToken cancellationToken)
    {
        var configuredPath =
            _configuration[configurationKey];

        if (string.IsNullOrWhiteSpace(configuredPath))
        {
            return ServiceResult<IReadOnlyCollection<T>>
                .Failure(
                    $"No se encontró {configurationKey}.");
        }

        var fullPath = Path.IsPathRooted(configuredPath)
            ? configuredPath
            : Path.Combine(
                _environment.ContentRootPath,
                configuredPath);

        if (!File.Exists(fullPath))
        {
            return ServiceResult<IReadOnlyCollection<T>>
                .Failure(
                    $"El archivo no existe: {fullPath}");
        }

        try
        {
            await using var stream =
                File.OpenRead(fullPath);

            var records =
                await JsonSerializer.DeserializeAsync<List<T>>(
                    stream,
                    _jsonOptions,
                    cancellationToken);

            records ??= new List<T>();

            _logger.LogInformation(
                "Se leyeron {RecordCount} registros desde {FilePath}.",
                records.Count,
                fullPath);

            return ServiceResult<IReadOnlyCollection<T>>
                .Success(records);
        }
        catch (JsonException exception)
        {
            _logger.LogError(
                exception,
                "El contenido JSON no es válido: {FilePath}.",
                fullPath);

            return ServiceResult<IReadOnlyCollection<T>>
                .Failure(
                    $"El JSON no es válido: {fullPath}");
        }
        catch (IOException exception)
        {
            _logger.LogError(
                exception,
                "No se pudo leer {FilePath}.",
                fullPath);

            return ServiceResult<IReadOnlyCollection<T>>
                .Failure(
                    $"No se pudo leer el archivo: {fullPath}");
        }
    }
}