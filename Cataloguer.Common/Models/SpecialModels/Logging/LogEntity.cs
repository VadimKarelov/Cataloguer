using System.ComponentModel.DataAnnotations;

namespace Cataloguer.Common.Models.SpecialModels.Logging;

public class LogEntity
{
    [Key]
    public int Id { get; set; }

    public DateTime DateTime { get; set; }

    public string TypeName { get; set; } = string.Empty;

    public int EntityId { get; set; }

    public string PropertyName { get; set; } = string.Empty;

    public string? PreviousValue { get; set; }

    public string? NewValue { get; set; }
}
