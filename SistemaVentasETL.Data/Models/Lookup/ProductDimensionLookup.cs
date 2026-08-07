namespace SistemaVentasETL.Data.Models.Lookup;

public sealed record ProductDimensionLookup(
    int IdProductoKey,
    int IdProductoOrigen,
    string NombreProducto,
    string? Categoria,
    decimal? Precio);