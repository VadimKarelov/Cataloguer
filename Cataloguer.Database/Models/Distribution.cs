using Cataloguer.Database.Base;
using Cataloguer.Database.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cataloguer.Database.Models
{
    [Table("distribution")]
    public class Distribution : ICataloguerModel<Distribution>
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

        [Column("age_group_id")]
        [JsonPropertyName("ageGroupId")]
        public int AgeGroupId { get; set; }
        [JsonIgnore]
        public AgeGroup? AgeGroup { get; set; }

        [Column("gender_id")]
        [JsonPropertyName("genderId")]
        public int GenderId { get; set; }
        [JsonIgnore]
        public Gender? Gender { get; set; }

        [Column("town_id")]
        [JsonPropertyName("townId")]
        public int TownId { get; set; }
        [JsonIgnore]
        public Town? Town { get; set; }

        [Column("brochure_count")]
        [JsonPropertyName("brochureCount")]
        public long BrochureCount { get; set; }

        Distribution? ICataloguerModel<Distribution>.Get(CataloguerApplicationContext context, int id, bool includeFields)
        {
            return !includeFields ?
                context.Distributions
                    .AsNoTracking()
                    .FirstOrDefault(x => x.Id == id) :
                context.Distributions
                    .AsNoTracking()
                    .Include(x => x.Brochure)
                    .Include(x => x.AgeGroup)
                    .Include(x => x.Gender)
                    .Include(x => x.Town)
                    .FirstOrDefault(x => x.Id == id);
        }

        IEnumerable<Distribution?>? ICataloguerModel<Distribution>.GetAll(CataloguerApplicationContext context, bool includeFields)
        {
            return !includeFields ?
                context.Distributions
                    .AsNoTracking()
                    .ToArray() :
                context.Distributions
                    .AsNoTracking()
                    .Include(x => x.Brochure)
                    .Include(x => x.AgeGroup)
                    .Include(x => x.Gender)
                    .Include(x => x.Town)
                    .ToArray();
        }

        IEnumerable<Distribution?>? ICataloguerModel<Distribution>.GetAll(CataloguerApplicationContext context, Func<Distribution, bool> predicate, bool includeFields)
        {
            return !includeFields ?
                context.Distributions
                    .AsNoTracking()
                    .Where(predicate)
                    .ToArray() :
                context.Distributions
                    .AsNoTracking()
                    .Include(x => x.Brochure)
                    .Include(x => x.AgeGroup)
                    .Include(x => x.Gender)
                    .Include(x => x.Town)
                    .Where(predicate)
                    .ToArray();
        }
    }
}
