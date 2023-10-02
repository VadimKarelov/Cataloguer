using System.ComponentModel.DataAnnotations.Schema;

namespace Cataloguer.Database.Models
{
    [Table("brochure")]
    public class Brochure
    {
        [Column("id")]
        public Guid Id { get; set; }

        [Column("status_id")]
        public Guid StatusId { get; set; }
        public Status? Status { get; set; }

        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("date")]
        public DateTime Date { get; set; }

        /// <summary>
        /// Тираж
        /// </summary>
        [Column("edition")]
        public int Edition { get; set; }

        [Column("position_count")]
        public int PositionCount { get; set; }
    }
}
