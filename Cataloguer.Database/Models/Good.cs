namespace Cataloguer.Database.Models
{
    /// <summary>
    /// Нет, не хорошо, а товар
    /// </summary>
    public class Good
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
