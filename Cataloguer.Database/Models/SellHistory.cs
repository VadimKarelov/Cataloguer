﻿using System.ComponentModel.DataAnnotations.Schema;

namespace Cataloguer.Database.Models
{
    [Table("sell_history")]
    public class SellHistory
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("good_id")]
        public Guid GoodId { get; set; }
        public Good? Good { get; set; }

        [Column("id")]
        public Guid TownId { get; set; }
        public Town? Town { get; set; }

        [Column("age")]
        public int Age { get; set; }

        [Column("sell_date")]
        public DateTime SellDate { get; set; }

        [Column("good_count")]
        public int GoodCount { get; set; }

        [Column("price")]
        public decimal Price { get; set; }
    }
}