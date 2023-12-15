using System.Text.Json.Serialization;

namespace Cataloguer.Common.Models.SpecialModels.OutputApiModels;

public class FrontendBrochure : Brochure
{
    public FrontendBrochure(Brochure brochure)
    {
        this.Id = brochure.Id;
        this.StatusId = brochure.StatusId;
        this.Status = brochure.Status;
        this.Date = brochure.Date;
        this.Edition = brochure.Edition;
        this.Name = brochure.Name;
        this.PositionCount = brochure.PositionCount;
        this.PotentialIncome = Math.Round(brochure.PotentialIncome, 2, MidpointRounding.ToZero);
    }

    [JsonPropertyName("statusName")]
    public string StatusName { get; set; } = string.Empty;
}
