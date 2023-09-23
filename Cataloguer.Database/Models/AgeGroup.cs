namespace Cataloguer.Database.Models
{
    public class AgeGroup
    {
        public Guid Id { get; set; }
        /// <summary>
        /// Содержит информацию о возрастной групппе, можно будет добавить set
        /// </summary>
        public string Description => $"Включает в себя клиентов возрастом от {MinimalAge} до {MaximalAge} лет, включительно со всех сторон.";
        /// <summary>
        /// Нижняя граница группы в годах, включительно
        /// </summary>
        public short MinimalAge { get; set; }
        /// <summary>
        /// Верхняя граница группы в годах, включительно
        /// </summary>
        public short MaximalAge { get; set; }
    }
}
