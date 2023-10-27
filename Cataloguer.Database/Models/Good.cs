using Cataloguer.Database.Base;
using Cataloguer.Database.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cataloguer.Database.Models
{
    /// <summary>
    /// Нет, не хорошо, а товар
    /// </summary>
    [Table("good")]
    public class Good : ICataloguerModel<Good>
    {
        [Key]
        [Column("id")]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [Column("name")]
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        Good? ICataloguerModel<Good>.Get(CataloguerApplicationContext context, int id, bool includeFields)
        {
            return context.Goods
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == id);
        }

        IEnumerable<Good?>? ICataloguerModel<Good>.GetAll(CataloguerApplicationContext context, bool includeFields)
        {
            return context.Goods
                .AsNoTracking()
                .ToArray();
        }

        IEnumerable<Good?>? ICataloguerModel<Good>.GetAll(CataloguerApplicationContext context, Func<Good, bool> predicate, bool includeFields)
        {
            return context.Goods
                .AsNoTracking()
                .ToArray();
        }
    }
}
