namespace SistemaVentasETL.Data.Interfaces;

public interface ITemporaryFileRepository
{
    Task SaveJsonAsync<T>(
        string fileName,
        IReadOnlyCollection<T> data,
        CancellationToken cancellationToken = default);
}

