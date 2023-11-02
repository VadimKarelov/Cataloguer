using System.Text.Json.Serialization;

namespace Cataloguer.Database.Models.SpecialModels.OutputApiModels
{
    public class FrontendGood : Good
    {
        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        public FrontendGood(Good good)
        {
            Id = good.Id;
            Name = good.Name;
        }
    }
}
