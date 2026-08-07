using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaVentasETL.Data.Models.Dimensions;

[Table("DimCliente", Schema = "dim")]
public sealed class DimCliente
{
    [Key]
    public int IdClienteKey { get; set; }

    public int IdClienteOrigen { get; set; }

    [MaxLength(150)]
    public string Nombre { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? TipoCliente { get; set; }

    [MaxLength(100)]
    public string? Ciudad { get; set; }

    [MaxLength(100)]
    public string? Region { get; set; }

    [MaxLength(100)]
    public string? Pais { get; set; }

    public DateTime FechaInicio { get; set; }

    public DateTime? FechaFin { get; set; }

    public bool EsActual { get; set; }
}