using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Cataloguer.Common.Models.SpecialModels.Logging;

public class LogEntity
{
    [Key]
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [JsonPropertyName("dateTime")]
    public DateTime DateTime { get; set; }

    [JsonPropertyName("typeName")]
    public string TypeName { get; set; } = string.Empty;

    [JsonPropertyName("entityId")]
    public int EntityId { get; set; }

    [JsonPropertyName("propertyName")]
    public string PropertyName { get; set; } = string.Empty;

    [JsonPropertyName("previousValue")]
    public string? PreviousValue { get; set; }

    [JsonPropertyName("newValue")]
    public string? NewValue { get; set; }
}
