using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cataloguer.Database.Models
{
    [Table("brochure_position")]
    public class BrochurePosition
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

        [Column("good_id")]
        [JsonPropertyName("goodId")]
        public int GoodId { get; set; }
        [JsonIgnore]
        public Good? Good { get; set; }

        [Column("price")]
        [JsonPropertyName("price")]
        public decimal Price { get; set; }
    }
}
