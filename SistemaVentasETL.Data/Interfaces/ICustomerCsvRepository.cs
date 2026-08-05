using SistemaVentasETL.Data.Models.Csv;

namespace SistemaVentasETL.Data.Interfaces;

public interface ICustomerCsvRepository
{
    Task<IReadOnlyCollection<CustomerCsv>> GetCustomersAsync(
        CancellationToken cancellationToken = default);
}

