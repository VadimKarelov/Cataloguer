using Cataloguer.Database.Base;
using Cataloguer.Database.Models.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text.Json.Serialization;

namespace Cataloguer.Database.Models
{
    [Table("sell_history")]
    public class SellHistory : ICataloguerModel<SellHistory>
    {
        [Key]
        [Column("id")]
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [Column("good_id")]
        [JsonPropertyName("goodId")]
        public int GoodId { get; set; }
        [JsonIgnore]
        public Good? Good { get; set; }

        [Column("town_id")]
        [JsonPropertyName("townId")]
        public int TownId { get; set; }
        [JsonIgnore]
        public Town? Town { get; set; }

        [Column("age")]
        [JsonPropertyName("age")]
        public short Age { get; set; }

        [Column("gender_id")]
        [JsonPropertyName("genderId")]
        public int GenderId { get; set; }
        [JsonIgnore]
        public Gender? Gender { get; set; }

        [Column("sell_date")]
        [JsonPropertyName("sellDate")]
        public DateTime SellDate { get; set; }

        /// <summary>
        /// Количество купленных единиц товара в одной записи
        /// </summary>
        [Column("good_count")]
        [JsonPropertyName("goodCount")]
        public int GoodCount { get; set; }

        [Column("price")]
        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        SellHistory? ICataloguerModel<SellHistory>.Get(CataloguerApplicationContext context, int id, bool includeFields)
        {
            return !includeFields ?
                context.SellHistory
                    .AsNoTracking()
                    .FirstOrDefault(x => x.Id == id) :
                context.SellHistory
                    .AsNoTracking()
                    .Include(x => x.Town)
                    .Include(x => x.Gender)
                    .Include(x => x.Good)
                    .FirstOrDefault(x => x.Id == id);
        }

        IEnumerable<SellHistory?>? ICataloguerModel<SellHistory>.GetAll(CataloguerApplicationContext context, bool includeFields)
        {
            return !includeFields ?
                context.SellHistory
                    .AsNoTracking()
                    .ToArray() :
                context.SellHistory
                    .AsNoTracking()
                    .Include(x => x.Town)
                    .Include(x => x.Gender)
                    .Include(x => x.Good)
                    .ToArray();
        }

        IEnumerable<SellHistory?>? ICataloguerModel<SellHistory>.GetAll(CataloguerApplicationContext context, Func<SellHistory, bool> predicate, bool includeFields)
        {
            return !includeFields ?
                context.SellHistory
                    .AsNoTracking()
                    .Where(predicate)
                    .ToArray() :
                context.SellHistory
                    .AsNoTracking()
                    .Include(x => x.Town)
                    .Include(x => x.Gender)
                    .Include(x => x.Good)
                    .Where(predicate)
                    .ToArray();
        }
    }
}
