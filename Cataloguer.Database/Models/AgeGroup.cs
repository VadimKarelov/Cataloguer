using System.ComponentModel.DataAnnotations.Schema;

namespace Cataloguer.Database.Models
{
    [Table("age_group")]
    public class AgeGroup
    {
        [Column("id")]
        public Guid Id { get; set; }

        /// <summary>
        /// Содержит информацию о возрастной групппе, можно будет добавить set
        /// </summary>
        [Column("description")]
        public string Description { get; set; } = string.Empty;

        /// <summary>
        /// Нижняя граница группы в годах, включительно
        /// </summary>
        [Column("min_age")]
        public short MinimalAge { get; set; }

        /// <summary>
        /// Верхняя граница группы в годах, включительно
        /// </summary>
        [Column("max_age")]
        public short MaximalAge { get; set; }
    }
}
