﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cataloguer.Common.Models;

/// <summary>
/// Нет, не хорошо, а товар
/// </summary>
[Table("good")]
public class Good
{
    [Key]
    [Column("id")]
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [Column("name")]
    [JsonPropertyName("name")]
    public string Name { get; set; } = string.Empty;
    
    [Column("price")]
    [JsonPropertyName("price")]
    public decimal Price { get; set; }
}