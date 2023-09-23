namespace Cataloguer.Database.Models
{
    public class Town
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public long Population { get; set; }
    }
}
