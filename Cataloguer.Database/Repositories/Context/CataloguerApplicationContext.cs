using Cataloguer.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Cataloguer.Database.Repositories.Context
{
    /// <summary>
    /// Можно использовать только внутри проекта Database
    /// </summary>
    internal class CataloguerApplicationContext : DbContext
    {
        public DbSet<AgeGroup> AgeGroups { get; set; }
        public DbSet<Brochure> Brochures { get; set; }
        public DbSet<BrochurePosition> BrochurePositions { get; set; }
        public DbSet<Distribution> Distributions { get; set; }
        public DbSet<Gender> Genders { get; set; }
        public DbSet<Good> Goods { get; set; }
        public DbSet<SellHistory> SellHistory { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<Town> Towns { get; set; }   

        public CataloguerApplicationContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // не стоит менять порядок инициализации

            modelBuilder.Entity<Town>().HasData(ReadTownsFromFile());

            modelBuilder.Entity<Good>().HasData(DoWithNotification(ReadGoodsFromFile, "Чтение списка продуктов"));

            modelBuilder.Entity<AgeGroup>().HasData(new AgeGroup[]
                {
                    new AgeGroup() { Description = "Дети", MinimalAge = 0, MaximalAge = 14 },
                    new AgeGroup() { Description = "Подростки", MinimalAge = 15, MaximalAge = 20 },
                    new AgeGroup() { Description = "Молодые", MinimalAge = 21, MaximalAge = 35 },
                    new AgeGroup() { Description = "Взрослые", MinimalAge = 36, MaximalAge = 50 },
                    new AgeGroup() { Description = "Пожилые", MinimalAge = 51, MaximalAge = short.MaxValue },
                });

            modelBuilder.Entity<Gender>().HasData(new Gender[]
                {
                    new Gender() { Name = "Мужской" },
                    new Gender() { Name = "Женский" },
                    new Gender() { Name = "Боевой вертолет" }
                });
        }

        private Town[] ReadTownsFromFile()
        {
            Random random = new Random();

            using StreamReader reader = new StreamReader("..\\..\\..\\Resources\\goroda.txt");
            return reader.ReadToEnd()
                .Split()
                .Where(x => !string.IsNullOrEmpty(x) && !x.Contains("Оспаривается"))
                .Select(x => new Town() { Name = x, Population = random.Next(5000, 20000000) })
                .ToArray();
        }

        private Good[] ReadGoodsFromFile()
        {
            Random random = new Random();

            using StreamReader reader = new StreamReader("..\\..\\..\\Resources\\goods.txt");
            return reader.ReadToEnd()
                .Split()
                .Where(x => !string.IsNullOrEmpty(x))
                .Distinct()
                .Select(x => new Good() { Name = x })
                .ToArray();
        }

        private T DoWithNotification<T>(Func<T> func, string funcName)
        {
            Console.WriteLine($"Начало выполнения метода: {funcName}");
            var res = func();
            Console.WriteLine($"Начало выполнения метода: {funcName}");
            return res;
        }
    }
}
