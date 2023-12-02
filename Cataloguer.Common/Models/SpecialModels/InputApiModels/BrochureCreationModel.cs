using System.Text.Json.Serialization;

namespace Cataloguer.Common.Models.SpecialModels.InputApiModels;

public class BrochureCreationModel
{
    [JsonPropertyName("name")] public string Name { get; set; }

    [JsonPropertyName("edition")] public int Edition { get; set; }

    [JsonPropertyName("date")] public DateTime Date { get; set; }

    [JsonPropertyName("positions")] public CreationPosition[] Positions { get; set; }
}
