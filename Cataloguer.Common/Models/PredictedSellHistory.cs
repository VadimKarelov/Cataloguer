using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
    
    public Brochure? Brochure { get; set; }
    
    [Column("prediction_date")]
    public DateTime PredictionDate { get; set; }
    
    /// <summary>
    /// Предсказанное значение
    /// </summary>
    [Column("value")]
    public decimal Value { get; set; }
}