using VentasAnalytics.Data.Models.Csv;

namespace VentasAnalytics.Data.Interfaces;

public interface ICustomerCsvRepository
{
    Task<IReadOnlyCollection<CustomerCsv>> GetCustomersAsync(
        CancellationToken cancellationToken = default);
}