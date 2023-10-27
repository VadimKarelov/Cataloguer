using Cataloguer.Database.Base;
using Cataloguer.Database.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cataloguer.Database.Models
{
    [Table("age_group")]
    public class AgeGroup : ICataloguerModel<AgeGroup>
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

        AgeGroup? ICataloguerModel<AgeGroup>.Get(CataloguerApplicationContext context, int id, bool includeFields)
        {
            return context.AgeGroups.FirstOrDefault(x => x.Id == id);
        }

        IEnumerable<AgeGroup?>? ICataloguerModel<AgeGroup>.GetAll(CataloguerApplicationContext context, bool includeFields)
        {
            return context.AgeGroups
                .AsNoTracking()
                .ToArray();
        }

        IEnumerable<AgeGroup?>? ICataloguerModel<AgeGroup>.GetAll(CataloguerApplicationContext context, Func<AgeGroup, bool> predicate, bool includeFields)
        {
            return context.AgeGroups
                .AsNoTracking()
                .Where(predicate)
                .ToArray();
        }
    }
}
