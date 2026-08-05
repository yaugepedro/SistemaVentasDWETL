using System.Diagnostics;
using VentasAnalytics.Data.Interfaces;

namespace VentasAnalytics.Load;

public sealed class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly ISalesRepository _salesRepository;
    private readonly IProductApiRepository _productApiRepository;
    private readonly IHostApplicationLifetime _applicationLifetime;
    private readonly ITemporaryFileRepository _temporaryFileRepository;
    private readonly ICustomerCsvRepository _customerCsvRepository;

    public Worker(
    ILogger<Worker> logger,
    ISalesRepository salesRepository,
    IProductApiRepository productApiRepository,
    ICustomerCsvRepository customerCsvRepository,
    ITemporaryFileRepository temporaryFileRepository,
    IHostApplicationLifetime applicationLifetime)
    {
        _logger = logger;
        _salesRepository = salesRepository;
        _productApiRepository = productApiRepository;
        _customerCsvRepository = customerCsvRepository;
        _temporaryFileRepository = temporaryFileRepository;
        _applicationLifetime = applicationLifetime;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        var totalStopwatch = Stopwatch.StartNew();

        try
        {
            await ExtractSalesAsync(stoppingToken);
            await ExtractProductsFromApiAsync(stoppingToken);
            await ExtractCustomersFromCsvAsync(stoppingToken);

            totalStopwatch.Stop();

            _logger.LogInformation(
                "Todas las extracciones finalizaron correctamente.");

            _logger.LogInformation(
                "Tiempo total: {ElapsedMilliseconds} ms",
                totalStopwatch.ElapsedMilliseconds);
        }
        catch (OperationCanceledException)
            when (stoppingToken.IsCancellationRequested)
        {
            _logger.LogWarning(
                "El proceso de extracción fue cancelado.");
        }
        catch (Exception exception)
        {
            _logger.LogError(
                exception,
                "Ocurrió un error durante el proceso de extracción.");
        }
        finally
        {
            _logger.LogInformation(
                "El proceso finalizará en 10 segundos.");

            await Task.Delay(
                TimeSpan.FromSeconds(10),
                CancellationToken.None);

            _applicationLifetime.StopApplication();
        }
    }

    private async Task ExtractSalesAsync(
        CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        _logger.LogInformation(
            "Iniciando la extracción de ventas con ADO.NET.");

        var sales =
            await _salesRepository.GetSalesAsync(cancellationToken);

        await _temporaryFileRepository.SaveJsonAsync(
            "ventas-db.json",
            sales,
            cancellationToken);

        _logger.LogInformation(
            "Las ventas fueron guardadas en Temp/ventas-db.json.");

        stopwatch.Stop();

        _logger.LogInformation(
            "Ventas extraídas: {RecordCount}",
            sales.Count);

        _logger.LogInformation(
            "Tiempo de extracción de ventas: {ElapsedMilliseconds} ms",
            stopwatch.ElapsedMilliseconds);
    }

    private async Task ExtractProductsFromApiAsync(
        CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        _logger.LogInformation(
            "Iniciando la extracción de productos desde la API REST.");

        var products =
            await _productApiRepository.GetProductsAsync(
                cancellationToken);

        await _temporaryFileRepository.SaveJsonAsync(
            "productos-api.json",
            products,
            cancellationToken);

        _logger.LogInformation(
            "Los productos fueron guardados en Temp/productos-api.json.");

        stopwatch.Stop();

        _logger.LogInformation(
            "Productos extraídos desde la API: {RecordCount}",
            products.Count);

        _logger.LogInformation(
            "Tiempo de extracción desde la API: {ElapsedMilliseconds} ms",
            stopwatch.ElapsedMilliseconds);
    }

    private async Task ExtractCustomersFromCsvAsync(
    CancellationToken cancellationToken)
    {
        var stopwatch = Stopwatch.StartNew();

        _logger.LogInformation(
            "Iniciando la extracción de clientes desde el archivo CSV.");

        var customers =
            await _customerCsvRepository.GetCustomersAsync(
                cancellationToken);

        await _temporaryFileRepository.SaveJsonAsync(
            "clientes-csv.json",
            customers,
            cancellationToken);

        stopwatch.Stop();

        _logger.LogInformation(
            "Los clientes fueron guardados en Temp/clientes-csv.json.");

        _logger.LogInformation(
            "Clientes extraídos desde CSV: {RecordCount}",
            customers.Count);

        _logger.LogInformation(
            "Tiempo de extracción desde CSV: {ElapsedMilliseconds} ms",
            stopwatch.ElapsedMilliseconds);
    }
}

