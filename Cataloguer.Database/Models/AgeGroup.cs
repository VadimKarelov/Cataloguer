using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cataloguer.Database.Models
{
    [Table("age_group")]
    public class AgeGroup
    {
        [Key]
        [Column("id")]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        /// <summary>
        /// Содержит информацию о возрастной групппе, можно будет добавить set
        /// </summary>
        [Column("description")]
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Нижняя граница группы в годах, включительно
        /// </summary>
        [Column("min_age")]
        [JsonPropertyName("minimalAge")]
        public short MinimalAge { get; set; }

        /// <summary>
        /// Верхняя граница группы в годах, включительно
        /// </summary>
        [Column("max_age")]
        [JsonPropertyName("maximalAge")]
        public short MaximalAge { get; set; }
    }
}
