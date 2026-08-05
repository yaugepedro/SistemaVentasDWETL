using SistemaVentasETL.Api.Models;

namespace SistemaVentasETL.Api.Repository;

public interface IProductRepository
{
    Task<IReadOnlyCollection<ProductResponse>> GetProductsAsync(
        CancellationToken cancellationToken = default);
}

