using SistemaVentasETL.Data.Interfaces;
using SistemaVentasETL.Data.Repositories.Api;
using SistemaVentasETL.Data.Repositories.Db;
using SistemaVentasETL.Data.Repositories.Files;
using SistemaVentasETL.Data.Repositories.Csv;
using SistemaVentasETL.Load;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<ISalesRepository>(serviceProvider =>
{
    var configuration =
        serviceProvider.GetRequiredService<IConfiguration>();

    var connectionString =
        configuration.GetConnectionString("Sistema_de_Ventas");

    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new InvalidOperationException(
            "No se encontrÃ³ la cadena de conexiÃ³n Sistema_de_Ventas.");
    }

    return new SalesRepository(connectionString);
});

var apiBaseUrl =
    builder.Configuration["ApiSettings:BaseUrl"];

if (string.IsNullOrWhiteSpace(apiBaseUrl))
{
    throw new InvalidOperationException(
        "No se encontrÃ³ la configuraciÃ³n ApiSettings:BaseUrl.");
}

builder.Services.AddHttpClient<
    IProductApiRepository,
    ProductApiRepository>(client =>
    {
        client.BaseAddress = new Uri(apiBaseUrl);
        client.Timeout = TimeSpan.FromSeconds(60);
    });

builder.Services.AddSingleton<ITemporaryFileRepository>(
    serviceProvider =>
    {
        var configuration =
            serviceProvider.GetRequiredService<IConfiguration>();

        var environment =
            serviceProvider.GetRequiredService<IHostEnvironment>();

        var directory =
            configuration["TemporaryFiles:Directory"];

        if (string.IsNullOrWhiteSpace(directory))
        {
            throw new InvalidOperationException(
                "No se encontrÃ³ TemporaryFiles:Directory.");
        }

        var fullPath = Path.Combine(
            environment.ContentRootPath,
            directory);

        return new JsonTemporaryFileRepository(fullPath);
    });

builder.Services.AddSingleton<ICustomerCsvRepository>(
    serviceProvider =>
    {
        var configuration =
            serviceProvider.GetRequiredService<IConfiguration>();

        var environment =
            serviceProvider.GetRequiredService<IHostEnvironment>();

        var relativePath =
            configuration["CsvSettings:CustomersPath"];

        if (string.IsNullOrWhiteSpace(relativePath))
        {
            throw new InvalidOperationException(
                "No se encontrÃ³ CsvSettings:CustomersPath.");
        }

        var fullPath = Path.Combine(
            environment.ContentRootPath,
            relativePath);

        return new CustomerCsvRepository(fullPath);
    });

builder.Services.AddHostedService<Worker>();

var host = builder.Build();

host.Run();

