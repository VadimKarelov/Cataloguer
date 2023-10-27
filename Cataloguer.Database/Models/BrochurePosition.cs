using Cataloguer.Database.Base;
using Cataloguer.Database.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cataloguer.Database.Models
{
    [Table("brochure_position")]
    public class BrochurePosition : ICataloguerModel<BrochurePosition>
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

        [Column("good_id")]
        [JsonPropertyName("goodId")]
        public int GoodId { get; set; }
        [JsonIgnore]
        public Good? Good { get; set; }

        [Column("price")]
        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        BrochurePosition? ICataloguerModel<BrochurePosition>.Get(CataloguerApplicationContext context, int id, bool includeFields)
        {
            return !includeFields ? 
                context.BrochurePositions
                    .AsNoTracking()
                    .FirstOrDefault(x => x.Id == id) :
                context.BrochurePositions
                    .AsNoTracking()
                    .Include(x => x.Brochure)
                    .Include(x => x.Good)
                    .FirstOrDefault(x => x.Id == id);
        }

        IEnumerable<BrochurePosition?>? ICataloguerModel<BrochurePosition>.GetAll(CataloguerApplicationContext context, bool includeFields)
        {
            return !includeFields ?
                context.BrochurePositions
                    .AsNoTracking()
                    .ToArray() :
                context.BrochurePositions
                    .AsNoTracking()
                    .Include(x => x.Brochure)
                    .Include(x => x.Good)
                    .ToArray();
        }

        IEnumerable<BrochurePosition?>? ICataloguerModel<BrochurePosition>.GetAll(CataloguerApplicationContext context, Func<BrochurePosition, bool> predicate, bool includeFields)
        {
            return !includeFields ?
                context.BrochurePositions
                    .AsNoTracking()
                    .Where(predicate)
                    .ToArray() :
                context.BrochurePositions
                    .AsNoTracking()
                    .Include(x => x.Brochure)
                    .Include(x => x.Good)
                    .Where(predicate)
                    .ToArray();
        }
    }
}
