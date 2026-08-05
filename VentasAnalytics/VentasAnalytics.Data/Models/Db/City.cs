#nullable disable
namespace VentasAnalytics.Data.Models.Db;

public partial class City
{
    public int CityId { get; set; }

    public string CityName { get; set; }

    public int CountryId { get; set; }
}