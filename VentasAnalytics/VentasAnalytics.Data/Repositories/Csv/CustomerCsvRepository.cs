using CsvHelper;
using System.Globalization;
using VentasAnalytics.Data.Interfaces;
using VentasAnalytics.Data.Models.Csv;

namespace VentasAnalytics.Data.Repositories.Csv;

public sealed class CustomerCsvRepository : ICustomerCsvRepository
{
    private readonly string _filePath;

    public CustomerCsvRepository(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException(
                "La ruta del CSV no puede estar vacía.",
                nameof(filePath));
        }

        _filePath = filePath;
    }

    public async Task<IReadOnlyCollection<CustomerCsv>> GetCustomersAsync(
        CancellationToken cancellationToken = default)
    {
        if (!File.Exists(_filePath))
        {
            throw new FileNotFoundException(
                $"No se encontró el archivo CSV: {_filePath}");
        }

        var customers = new List<CustomerCsv>();

        using var reader = new StreamReader(_filePath);

        using var csv = new CsvReader(
            reader,
            CultureInfo.InvariantCulture);

        await foreach (
            var customer in csv.GetRecordsAsync<CustomerCsv>(
                cancellationToken))
        {
            customers.Add(customer);
        }

        return customers;
    }
}