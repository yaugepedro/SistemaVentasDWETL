namespace SistemaVentasETL.Data.Models.Lookup;

public sealed record CustomerDimensionLookup(
    int IdClienteKey,
    int IdClienteOrigen,
    string Nombre,
    string? Ciudad,
    string? Pais);