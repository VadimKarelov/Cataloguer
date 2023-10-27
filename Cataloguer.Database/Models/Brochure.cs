using Cataloguer.Database.Base;
using Cataloguer.Database.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cataloguer.Database.Models
{
    [Table("brochure")]
    public class Brochure : ICataloguerModel<Brochure>
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

        /// <summary>
        /// Число товаров в каталоге
        /// </summary>
        [Column("position_count")]
        [JsonPropertyName("positionCount")]
        public int PositionCount { get; set; }

        Brochure? ICataloguerModel<Brochure>.Get(CataloguerApplicationContext context, int id, bool includeFields)
        {
            return !includeFields ?
                context.Brochures
                    .AsNoTracking()
                    .FirstOrDefault(x => x.Id == id) :
                context.Brochures
                    .AsNoTracking()
                    .Include(x => x.Status)
                    .FirstOrDefault(x => x.Id == id);
        }

        IEnumerable<Brochure?>? ICataloguerModel<Brochure>.GetAll(CataloguerApplicationContext context, bool includeFields)
        {
            return !includeFields ?
                context.Brochures
                    .AsNoTracking()
                    .ToArray() :
                context.Brochures
                    .AsNoTracking()
                    .Include(x => x.Status)
                    .ToArray();
        }

        IEnumerable<Brochure?>? ICataloguerModel<Brochure>.GetAll(CataloguerApplicationContext context, Func<Brochure, bool> predicate, bool includeFields)
        {
            return !includeFields ?
                context.Brochures
                    .AsNoTracking()
                    .Where(predicate)
                    .ToArray() :
                context.Brochures
                    .AsNoTracking()
                    .Include(x => x.Status)
                    .Where(predicate)
                    .ToArray();
        }
    }
}
