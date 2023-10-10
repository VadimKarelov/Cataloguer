using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cataloguer.Database.Models
{
    [Table("town")]
    public class Town
    {
        [Column("id")]
        public int Id { get; set; } = -1;

        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("population")]
        public long Population { get; set; }
    }
}
