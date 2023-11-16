using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cataloguer.Common.Models;

[Table("gender")]
public class Gender
{
    [Key]
    [Column("id")]
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [Column("name")]
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
}