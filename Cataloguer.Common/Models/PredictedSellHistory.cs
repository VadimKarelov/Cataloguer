using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Cataloguer.Common.Models;

/// <summary>
/// Хранит информацию о предсказанных значениях по продажам по каталогу
/// </summary>
[Table("predicted_sell_history")]
public class PredictedSellHistory
{
    [Key]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("brochure_id")]
    public int BrochureId { get; set; }
    
    [JsonIgnore] // нужен, чтобы это поле не отображалось в логах
    public Brochure? Brochure { get; set; }
    
    [Column("prediction_date")]
    public DateOnly PredictionDate { get; set; }
    
    /// <summary>
    /// Предсказанное значение
    /// </summary>
    [Column("value")]
    public decimal Value { get; set; }
}