using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cataloguer.Database.Models
{
    [Table("brochure")]
    public class Brochure
    {
        [Key]
        [Column("id")]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [Column("status_id")]
        [JsonPropertyName("statusId")]
        public int StatusId { get; set; }
        [JsonIgnore]
        public Status? Status { get; set; }

        [Column("name")]
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [Column("date")]
        [JsonPropertyName("date")]
        public DateTime Date { get; set; }

        /// <summary>
        /// Тираж
        /// </summary>
        [Column("edition")]
        [JsonPropertyName("edition")]
        public long Edition { get; set; }

        [Column("position_count")]
        [JsonPropertyName("positionCount")]
        public int PositionCount { get; set; }
    }
}
