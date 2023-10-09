using System.ComponentModel.DataAnnotations.Schema;

namespace Cataloguer.Database.Models
{
    [Table("sell_history")]
    public class SellHistory
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("good_id")]
        public Guid GoodId { get; set; }
        public Good? Good { get; set; }

        [Column("id")]
        public Guid TownId { get; set; }
        public Town? Town { get; set; }

        [Column("age")]
        public short Age { get; set; }

        [Column("sell_date")]
        public DateTime SellDate { get; set; }

        /// <summary>
        /// Количество купленных единиц товара в одной записи
        /// </summary>
        [Column("good_count")]
        public int GoodCount { get; set; }

        [Column("price")]
        public decimal Price { get; set; }
    }
}
