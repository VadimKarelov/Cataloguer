using Cataloguer.Database.Base;
using Cataloguer.Database.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cataloguer.Database.Models
{
    [Table("town")]
    public class Town : ICataloguerModel<Town>
    {
        [Key]
        [Column("id")]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [Column("name")]
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [Column("population")]
        [JsonPropertyName("population")]
        public long Population { get; set; }

        Town? ICataloguerModel<Town>.Get(CataloguerApplicationContext context, int id, bool includeFields)
        {
            return context.Towns
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == id);
        }

        IEnumerable<Town?>? ICataloguerModel<Town>.GetAll(CataloguerApplicationContext context, bool includeFields)
        {
            return context.Towns
                .AsNoTracking()
                .ToArray();
        }

        IEnumerable<Town?>? ICataloguerModel<Town>.GetAll(CataloguerApplicationContext context, Func<Town, bool> predicate, bool includeFields)
        {
            return context.Towns
                .AsNoTracking()
                .Where(predicate)
                .ToArray();
        }
    }
}
