using VentasAnalytics.Data.Models.Api;

namespace VentasAnalytics.Data.Interfaces;

public interface IProductApiRepository
{
    Task<IReadOnlyCollection<ProductApi>> GetProductsAsync(
        CancellationToken cancellationToken = default);
}