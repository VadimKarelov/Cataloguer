namespace Cataloguer.Database.Models
{
    public class SellHistory
    {
        public Guid Id { get; set; }
        public Good Good { get; set; }
        public Town Town { get; set; }
        public int Age { get; set; }
        public DateTime SellDate { get; set; }
        public int GoodCount { get; set; }
        public decimal Price { get; set; }
    }
}
