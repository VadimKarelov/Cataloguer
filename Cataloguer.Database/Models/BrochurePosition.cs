using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cataloguer.Database.Models
{
    [Table("brochure_position")]
    public class BrochurePosition
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("brochure_id")]
        public int BrochureId { get; set; }
        public Brochure? Brochure { get; set; }

        [Column("good_id")]
        public int GoodId { get; set; }
        public Good? Good { get; set; }

        [Column("price")]
        public decimal Price { get; set; }
    }
}
