using System.ComponentModel.DataAnnotations.Schema;

namespace Cataloguer.Database.Models
{
    [Table("distribution")]
    public class Distribution
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("brochure_id")]
        public Guid BrochureId { get; set; }
        public Brochure? Brochure { get; set; }

        [Column("age_group_id")]
        public Guid AgeGroupId { get; set; }
        public AgeGroup? AgeGroup { get; set; }

        [Column("gender_id")]
        public Guid GenderId { get; set; }
        public Gender? Gender { get; set; }

        [Column("town_id")]
        public Guid TownId { get; set; }
        public Town? Town { get; set; }

        [Column("brochure_count")]
        public long BrochureCount { get; set; }
    }
}
