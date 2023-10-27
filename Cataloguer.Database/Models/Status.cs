using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cataloguer.Database.Models
{
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
    }
}
