using SistemaVentasETL.Data.Models.Lookup;

namespace SistemaVentasETL.Data.Services.Interfaces;

public interface IDimensionLookupService
{
    Task<IReadOnlyDictionary<int, CustomerDimensionLookup>>
        GetCurrentCustomersAsync(
            CancellationToken cancellationToken = default);

    Task<IReadOnlyDictionary<int, ProductDimensionLookup>>
        GetCurrentProductsAsync(
            CancellationToken cancellationToken = default);
}