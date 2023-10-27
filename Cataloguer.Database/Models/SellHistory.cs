using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cataloguer.Database.Models
{
    [Table("sell_history")]
    public class SellHistory
    {
        [Key]
        [Column("id")]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [Column("good_id")]
        [JsonPropertyName("goodId")]
        public int GoodId { get; set; }
        [JsonIgnore]
        public Good? Good { get; set; }

        [Column("town_id")]
        [JsonPropertyName("townId")]
        public int TownId { get; set; }
        [JsonIgnore]
        public Town? Town { get; set; }

        [Column("age")]
        [JsonPropertyName("age")]
        public short Age { get; set; }

        [Column("gender_id")]
        [JsonPropertyName("genderId")]
        public int GenderId { get; set; }
        [JsonIgnore]
        public Gender? Gender { get; set; }

        [Column("sell_date")]
        [JsonPropertyName("sellDate")]
        public DateTime SellDate { get; set; }

        /// <summary>
        /// Количество купленных единиц товара в одной записи
        /// </summary>
        [Column("good_count")]
        [JsonPropertyName("goodCount")]
        public int GoodCount { get; set; }

        [Column("price")]
        [JsonPropertyName("price")]
        public decimal Price { get; set; }
    }
}
