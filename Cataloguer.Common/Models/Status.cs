using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cataloguer.Common.Models;

[Table("status")]
public class Status
{
    [Key]
    [Column("id")]
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [Column("name")]
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;

    public static Status[] AvailableStatuses = new Status[] { NotReleased, Released };

    public static Status NotReleased = new Status() { Id = 1, Name = "Не выпущен" };
    public static Status Released = new Status() { Id = 2, Name = "Выпущен" };
}