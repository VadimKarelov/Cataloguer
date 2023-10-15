using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cataloguer.Database.Models
{
    [Table("distribution")]
    public class Distribution
    {
        [Key]
        [Column("id")]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [Column("brochure_id")]
        [JsonPropertyName("brochureId")]
        public int BrochureId { get; set; }
        [JsonIgnore]
        public Brochure? Brochure { get; set; }

        [Column("age_group_id")]
        [JsonPropertyName("ageGroupId")]
        public int AgeGroupId { get; set; }
        [JsonIgnore]
        public AgeGroup? AgeGroup { get; set; }

        [Column("gender_id")]
        [JsonPropertyName("genderId")]
        public int GenderId { get; set; }
        [JsonIgnore]
        public Gender? Gender { get; set; }

        [Column("town_id")]
        [JsonPropertyName("townId")]
        public int TownId { get; set; }
        [JsonIgnore]
        public Town? Town { get; set; }

        [Column("brochure_count")]
        [JsonPropertyName("brochureCount")]
        public long BrochureCount { get; set; }
    }
}
