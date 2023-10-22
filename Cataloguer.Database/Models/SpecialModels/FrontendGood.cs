using Cataloguer.Database.Models;
using System.Text.Json.Serialization;

namespace Cataloguer.Server.FrontendModels
{
    public class FrontendGood : Good
    {
        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        public FrontendGood(Good good)
        {
            this.Id = good.Id;
            this.Name = good.Name;
        }
    }
}
