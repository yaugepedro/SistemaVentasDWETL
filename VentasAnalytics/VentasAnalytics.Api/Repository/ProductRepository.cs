using Microsoft.EntityFrameworkCore;
using VentasAnalytics.Api.Models;
using VentasAnalytics.Data.Context;

namespace VentasAnalytics.Api.Repository;

public sealed class ProductRepository : IProductRepository
{
    private readonly VentasDBContext _context;

    public ProductRepository(VentasDBContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<ProductResponse>> GetProductsAsync(
        CancellationToken cancellationToken = default)
    {
        var products =
            await (
                from product in _context.Products.AsNoTracking()
                join category in _context.Categories.AsNoTracking()
                    on product.CategoryId equals category.CategoryId
                orderby product.ProductId
                select new ProductResponse
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    CategoryId = category.CategoryId,
                    CategoryName = category.CategoryName,
                    Price = product.Price,
                    Stock = product.Stock
                }
            ).ToListAsync(cancellationToken);

        return products;
    }
}