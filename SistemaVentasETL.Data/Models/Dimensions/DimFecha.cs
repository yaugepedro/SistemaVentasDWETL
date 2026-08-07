using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaVentasETL.Data.Models.Dimensions;

[Table("DimFecha", Schema = "dim")]
public sealed class DimFecha
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public int IdFechaKey { get; set; }

    public DateTime Fecha { get; set; }

    public int Dia { get; set; }

    public int Mes { get; set; }

    public string? NombreMes { get; set; }

    public int Trimestre { get; set; }

    public int Anio { get; set; }

    public string? NombreDia { get; set; }

    public bool? EsFinDeSemana { get; set; }
}