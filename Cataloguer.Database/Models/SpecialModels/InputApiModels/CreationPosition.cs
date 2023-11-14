using System.Text.Json.Serialization;

namespace Cataloguer.Database.Models.SpecialModels.InputApiModels;

public class CreationPosition
{
    [JsonPropertyName("id")]
    public int GoodId { get; set; }

    [JsonPropertyName("price")]
    public decimal Price { get; set; }
}