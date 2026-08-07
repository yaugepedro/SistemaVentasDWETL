using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaVentasETL.Data.Models.Dimensions;

[Table("DimProducto", Schema = "dim")]
public sealed class DimProducto
{
    [Key]
    public int IdProductoKey { get; set; }

    public int IdProductoOrigen { get; set; }

    [MaxLength(150)]
    public string NombreProducto { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? Categoria { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal? Precio { get; set; }

    public DateTime FechaInicio { get; set; }

    public DateTime? FechaFin { get; set; }

    public bool EsActual { get; set; }
}