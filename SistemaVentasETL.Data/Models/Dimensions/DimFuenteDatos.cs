using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaVentasETL.Data.Models.Dimensions;

[Table("DimFuenteDatos", Schema = "dim")]
public sealed class DimFuenteDatos
{
    [Key]
    public int IdFuenteKey { get; set; }

    [MaxLength(100)]
    public string NombreFuente { get; set; } = string.Empty;

    [MaxLength(50)]
    public string TipoFuente { get; set; } = string.Empty;

    [MaxLength(250)]
    public string? Descripcion { get; set; }
}