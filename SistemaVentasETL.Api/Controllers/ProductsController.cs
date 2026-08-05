using Microsoft.AspNetCore.Mvc;
using SistemaVentasETL.Api.Models;
using SistemaVentasETL.Api.Repository;

namespace SistemaVentasETL.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class ProductsController : ControllerBase
{
    private readonly IProductRepository _productRepository;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(
        IProductRepository productRepository,
        ILogger<ProductsController> logger)
    {
        _productRepository = productRepository;
        _logger = logger;
    }

    [HttpGet]
    [ProducesResponseType(
        typeof(IReadOnlyCollection<ProductResponse>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyCollection<ProductResponse>>> Get(
        CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Consultando productos mediante Entity Framework Core.");

        var products =
            await _productRepository.GetProductsAsync(cancellationToken);

        _logger.LogInformation(
            "La API devolvió {ProductCount} productos.",
            products.Count);

        return Ok(products);
    }
}

