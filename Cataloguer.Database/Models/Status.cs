using Cataloguer.Database.Base;
using Cataloguer.Database.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cataloguer.Database.Models
{
    [Table("status")]
    public class Status : ICataloguerModel<Status>
    {
        [Key]
        [Column("id")]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [Column("name")]
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        Status? ICataloguerModel<Status>.Get(CataloguerApplicationContext context, int id, bool includeFields)
        {
            return context.Statuses
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == id);
        }

        IEnumerable<Status?>? ICataloguerModel<Status>.GetAll(CataloguerApplicationContext context, bool includeFields)
        {
            return context.Statuses
                .AsNoTracking()
                .ToArray();
        }

        IEnumerable<Status?>? ICataloguerModel<Status>.GetAll(CataloguerApplicationContext context, Func<Status, bool> predicate, bool includeFields)
        {
            return context.Statuses
                .AsNoTracking()
                .Where(predicate)
                .ToArray();
        }
    }
}
