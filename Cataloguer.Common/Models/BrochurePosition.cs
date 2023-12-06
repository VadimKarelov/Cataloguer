using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cataloguer.Common.Models;

[Table("brochure_position")]
public class BrochurePosition : IEquatable<BrochurePosition>
{
    [Key]
    [Column("id")]
    [JsonPropertyName("id")]
    public int Id { get; set; }

    [Column("brochure_id")]
    [JsonPropertyName("brochureId")]
    public int BrochureId { get; set; }

    [JsonIgnore] public Brochure? Brochure { get; set; }

    [Column("good_id")]
    [JsonPropertyName("goodId")]
    public int GoodId { get; set; }

    [JsonIgnore] public Good? Good { get; set; }

    [Column("price")]
    [JsonPropertyName("price")]
    public decimal Price { get; set; }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj.GetType() != this.GetType()) return false;
        return Equals((BrochurePosition)obj);
    }

    public bool Equals(BrochurePosition? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Id == other.Id &&
               BrochureId == other.BrochureId && 
               GoodId == other.GoodId && 
               Price == other.Price;
    }
}