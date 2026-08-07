using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaVentasETL.Data.Models.Dimensions;

[Table("DimEstadoOrden", Schema = "dim")]
public sealed class DimEstadoOrden
{
    [Key]
    public int IdEstadoKey { get; set; }

    public string Estado { get; set; } = string.Empty;
}