using VentasAnalytics.Data.Models.Db;

namespace VentasAnalytics.Data.Interfaces;

public interface ISalesRepository
{
    Task<IReadOnlyCollection<Sale>> GetSalesAsync(
        CancellationToken cancellationToken = default);
}