using VentasAnalytics.Data.Interfaces;
using VentasAnalytics.Data.Repositories.Api;
using VentasAnalytics.Data.Repositories.Db;
using VentasAnalytics.Data.Repositories.Files;
using VentasAnalytics.Data.Repositories.Csv;
using VentasAnalytics.Load;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddSingleton<ISalesRepository>(serviceProvider =>
{
    var configuration =
        serviceProvider.GetRequiredService<IConfiguration>();

    var connectionString =
        configuration.GetConnectionString("VentasDB");

    if (string.IsNullOrWhiteSpace(connectionString))
    {
        throw new InvalidOperationException(
            "No se encontró la cadena de conexión VentasDB.");
    }

    return new SalesRepository(connectionString);
});



var apiBaseUrl =
    builder.Configuration["ApiSettings:BaseUrl"];

if (string.IsNullOrWhiteSpace(apiBaseUrl))
{
    throw new InvalidOperationException(
        "No se encontró la configuración ApiSettings:BaseUrl.");
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
                "No se encontró TemporaryFiles:Directory.");
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
                "No se encontró CsvSettings:CustomersPath.");
        }

        var fullPath = Path.Combine(
            environment.ContentRootPath,
            relativePath);

        return new CustomerCsvRepository(fullPath);
    });

builder.Services.AddHostedService<Worker>();

var host = builder.Build();

host.Run();