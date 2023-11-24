using System.Text.Json.Serialization;

namespace Cataloguer.Common.Models.SpecialModels.OutputApiModels;

public class SellHistoryForChart
{
    [JsonPropertyName("date")]
    public DateTime Date { get; set; }
    
    [JsonPropertyName("income")]
    public decimal Income { get; set; }
}