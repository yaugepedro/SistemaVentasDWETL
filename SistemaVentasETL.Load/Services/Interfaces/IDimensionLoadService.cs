using SistemaVentasETL.Load.Utilities;

namespace SistemaVentasETL.Load.Services.Interfaces;

public interface IDimensionLoadService
{
    Task<ServiceResult<int>> LoadDimensionsAsync(
        CancellationToken cancellationToken = default);
}