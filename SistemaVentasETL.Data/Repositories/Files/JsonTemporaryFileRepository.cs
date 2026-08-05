using System.Text.Json;
using SistemaVentasETL.Data.Interfaces;

namespace SistemaVentasETL.Data.Repositories.Files;

public sealed class JsonTemporaryFileRepository
    : ITemporaryFileRepository
{
    private readonly string _outputDirectory;

    public JsonTemporaryFileRepository(string outputDirectory)
    {
        if (string.IsNullOrWhiteSpace(outputDirectory))
        {
            throw new ArgumentException(
                "La ruta de salida no puede estar vacía.",
                nameof(outputDirectory));
        }

        _outputDirectory = outputDirectory;
    }

    public async Task SaveJsonAsync<T>(
        string fileName,
        IReadOnlyCollection<T> data,
        CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(_outputDirectory);

        var filePath = Path.Combine(
            _outputDirectory,
            fileName);

        await using var stream = File.Create(filePath);

        await JsonSerializer.SerializeAsync(
            stream,
            data,
            new JsonSerializerOptions
            {
                WriteIndented = true
            },
            cancellationToken);
    }
}

