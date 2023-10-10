using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cataloguer.Database.Models
{
    /// <summary>
    /// Нет, не хорошо, а товар
    /// </summary>
    [Table("good")]
    public class Good
    {
        [Column("id")]
        public int Id { get; set; } = -1;

        [Column("name")]
        public string Name { get; set; } = string.Empty;
    }
}
