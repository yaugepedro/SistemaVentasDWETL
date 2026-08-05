using System.Net.Http.Json;
using SistemaVentasETL.Data.Interfaces;
using SistemaVentasETL.Data.Models.Api;

namespace SistemaVentasETL.Data.Repositories.Api;

public sealed class ProductApiRepository : IProductApiRepository
{
    private readonly HttpClient _httpClient;

    public ProductApiRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<IReadOnlyCollection<ProductApi>> GetProductsAsync(
        CancellationToken cancellationToken = default)
    {
        using var response = await _httpClient.GetAsync(
            "api/Products",
            cancellationToken);

        response.EnsureSuccessStatusCode();

        var products =
            await response.Content.ReadFromJsonAsync<List<ProductApi>>(
                cancellationToken: cancellationToken);

        return products ?? [];
    }
}

