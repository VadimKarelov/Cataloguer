using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cataloguer.Database.Models
{
    [Table("distribution")]
    public class Distribution
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Column("brochure_id")]
        public int BrochureId { get; set; }
        public Brochure? Brochure { get; set; }

        [Column("age_group_id")]
        public int AgeGroupId { get; set; }
        public AgeGroup? AgeGroup { get; set; }

        [Column("gender_id")]
        public int GenderId { get; set; }
        public Gender? Gender { get; set; }

        [Column("town_id")]
        public int TownId { get; set; }
        public Town? Town { get; set; }

        [Column("brochure_count")]
        public long BrochureCount { get; set; }
    }
}
