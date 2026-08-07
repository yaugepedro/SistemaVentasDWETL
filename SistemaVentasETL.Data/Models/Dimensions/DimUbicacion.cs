using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaVentasETL.Data.Models.Dimensions;

[Table("DimUbicacion", Schema = "dim")]
public sealed class DimUbicacion
{
    [Key]
    public int IdUbicacionKey { get; set; }

    public string? Ciudad { get; set; }

    public string? Region { get; set; }

    public string? Pais { get; set; }
}