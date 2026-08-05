using SistemaVentasETL.Data.Models.Db;

namespace SistemaVentasETL.Data.Interfaces;

public interface ISalesRepository
{
    Task<IReadOnlyCollection<Sale>> GetSalesAsync(
        CancellationToken cancellationToken = default);
}

