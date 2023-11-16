using System.Text.Json.Serialization;

namespace Cataloguer.Common.Models.SpecialModels.OutputApiModels;

public class FrontendDistribution : Distribution
{
    public FrontendDistribution(Distribution distribution)
    {
        Id = distribution.Id;
        BrochureId = distribution.BrochureId;
        AgeGroupId = distribution.AgeGroupId;
        GenderId = distribution.GenderId;
        TownId = distribution.TownId;
        BrochureCount = distribution.BrochureCount;
    }

    [JsonPropertyName("brochureName")] public string BrochureName { get; set; } = string.Empty;

    [JsonPropertyName("ageGroupName")] public string AgeGroupName { get; set; } = string.Empty;

    [JsonPropertyName("genderName")] public string GenderName { get; set; } = string.Empty;

    [JsonPropertyName("townName")] public string TownName { get; set; } = string.Empty;
}