namespace Cataloguer.Database.Models
{
    public class Brochure
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        /// <summary>
        /// Тираж
        /// </summary>
        public int Edition { get; set; }
        public int PositionCount { get; set; }
    }
}
