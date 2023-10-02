using System.ComponentModel.DataAnnotations.Schema;

namespace Cataloguer.Database.Models
{
    [Table("brochure_position")]
    public class BrochurePosition
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("brochure_id")]
        public Guid BrochureId { get; set; }
        public Brochure? Brochure { get; set; }

        [Column("good_id")]
        public Guid GoodId { get; set; }
        public Good? Good { get; set; }

        [Column("price")]
        public int Price { get; set; }
    }
}
