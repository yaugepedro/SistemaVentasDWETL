using SistemaVentasETL.Data.Models.Api;

namespace SistemaVentasETL.Data.Interfaces;

public interface IProductApiRepository
{
    Task<IReadOnlyCollection<ProductApi>> GetProductsAsync(
        CancellationToken cancellationToken = default);
}

