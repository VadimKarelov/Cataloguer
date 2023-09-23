namespace Cataloguer.Database.Models
{
    public class Distribution
    {
        public Guid Id { get; set; }
        public Brochure Brochure { get; set; }
        public AgeGroup AgeGroup { get; set; }
        public Gender Gender { get; set; }
        public Town Town { get; set; }
        public long BrochureCount { get; set; }
    }
}
