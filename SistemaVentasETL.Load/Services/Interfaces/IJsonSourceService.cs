using SistemaVentasETL.Data.Models.Staging;
using SistemaVentasETL.Load.Utilities;

namespace SistemaVentasETL.Load.Services.Interfaces;

public interface IJsonSourceService
{
    Task<ServiceResult<IReadOnlyCollection<SaleStaging>>>
        GetSalesAsync(
            CancellationToken cancellationToken = default);

    Task<ServiceResult<IReadOnlyCollection<ProductStaging>>>
        GetProductsAsync(
            CancellationToken cancellationToken = default);

    Task<ServiceResult<IReadOnlyCollection<CustomerStaging>>>
        GetCustomersAsync(
            CancellationToken cancellationToken = default);
}