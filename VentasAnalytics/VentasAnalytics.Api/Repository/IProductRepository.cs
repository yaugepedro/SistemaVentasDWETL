using VentasAnalytics.Api.Models;

namespace VentasAnalytics.Api.Repository;

public interface IProductRepository
{
    Task<IReadOnlyCollection<ProductResponse>> GetProductsAsync(
        CancellationToken cancellationToken = default);
}