using System.Text.Json.Serialization;

namespace Cataloguer.Common.Models.SpecialModels.OutputApiModels;

public class FrontendGood : Good
{
    public FrontendGood(Good good)
    {
        Id = good.Id;
        Name = good.Name;
    }

    [JsonPropertyName("price")] public decimal Price { get; set; }
}