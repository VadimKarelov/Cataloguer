﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Cataloguer.Database.Models
{
    [Table("status")]
    public class Status
    {
        [Column("id")]
        public int Id { get; set; } = -1;

        [Column("name")]
        public string Name { get; set; } = string.Empty;
    }
}
