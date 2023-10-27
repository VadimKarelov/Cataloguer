using Cataloguer.Database.Base;
using Cataloguer.Database.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cataloguer.Database.Models
{
    [Table("gender")]
    public class Gender : ICataloguerModel<Gender>
    {
        [Key]
        [Column("id")]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [Column("name")]
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        Gender? ICataloguerModel<Gender>.Get(CataloguerApplicationContext context, int id, bool includeFields)
        {
            return context.Genders
                .AsNoTracking()
                .FirstOrDefault(x => x.Id == id);
        }

        IEnumerable<Gender?>? ICataloguerModel<Gender>.GetAll(CataloguerApplicationContext context, bool includeFields)
        {
            return context.Genders
                .AsNoTracking()
                .ToArray();
        }

        IEnumerable<Gender?>? ICataloguerModel<Gender>.GetAll(CataloguerApplicationContext context, Func<Gender, bool> predicate, bool includeFields)
        {
            return context.Genders
                .AsNoTracking()
                .ToArray();
        }
    }
}
