namespace Cataloguer.Database.Models
{
    public class BrochurePosition
    {
        public Guid Id { get; set; }
        public Brochure Brochure { get; set; }
        public Good Good { get; set; }
        public int Price { get; set; }
    }
}
