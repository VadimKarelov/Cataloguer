using System.Text.Json.Serialization;

namespace Cataloguer.Database.Models.SpecialModels.OutputApiModels
{
    public class FrontendDistribution : Distribution
    {
        [JsonPropertyName("brochureName")]
        public string BrochureName { get; set; } = string.Empty;

        [JsonPropertyName("ageGroupName")]
        public string AgeGroupName { get; set; } = string.Empty;

        [JsonPropertyName("genderName")]
        public string GenderName { get; set; } = string.Empty;

        [JsonPropertyName("townName")]
        public string TownName { get; set; } = string.Empty;

        public FrontendDistribution(Distribution distribution)
        {
            this.Id = distribution.Id;
            this.BrochureId = distribution.BrochureId;
            this.AgeGroupId = distribution.AgeGroupId;
            this.GenderId = distribution.GenderId;
            this.TownId = distribution.TownId;
            this.BrochureCount = distribution.BrochureCount;
        }
    }
}
