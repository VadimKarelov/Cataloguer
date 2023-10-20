using Cataloguer.Database.Models;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace Cataloguer.Database.Base
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

        private string _connectionString;

        public CataloguerApplicationContext(DataBaseConfiguration config)
        {
            _connectionString = config.ConnectionsString;

            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //todo вынести в конфиг
            optionsBuilder.UseNpgsql(_connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AgeGroup>().HasData(new AgeGroup[]
                {
                    new AgeGroup() { Id = 1, Description = "Дети", MinimalAge = 0, MaximalAge = 14 },
                    new AgeGroup() { Id = 2, Description = "Подростки", MinimalAge = 15, MaximalAge = 20 },
                    new AgeGroup() { Id = 3, Description = "Молодые", MinimalAge = 21, MaximalAge = 35 },
                    new AgeGroup() { Id = 4, Description = "Взрослые", MinimalAge = 36, MaximalAge = 50 },
                    new AgeGroup() { Id = 5, Description = "Пожилые", MinimalAge = 51, MaximalAge = short.MaxValue },
                });

            modelBuilder.Entity<Gender>().HasData(new Gender[]
                {
                    new Gender() { Id = 1, Name = "Мужской" },
                    new Gender() { Id = 2, Name = "Женский" },
                    new Gender() { Id = 3, Name = "Боевой вертолет" }
                });

            modelBuilder.Entity<Status>().HasData(new Status[]
                {
                    new Status() { Id = 1, Name = "Не проверено" },
                    new Status() { Id = 2, Name = "Эффективный" },
                    new Status() { Id = 3, Name = "Не эффективный" },
                });

            modelBuilder.Entity<SellHistory>().HasKey(x => x.Id);
        }

        public void Init()
        {
            if (!Towns.Any())
            {
                Towns.AddRange(ReadTownsFromFile());
                this.SaveChanges();
            }

            if (!Goods.Any())
            {
                Goods.AddRange(DoWithNotification(ReadGoodsFromFile, "Чтение списка продуктов"));
                this.SaveChanges();
            }

            if (!SellHistory.Any())
            {
                SellHistory.AddRange(DoWithNotification(GenerateSellHistory, "Создание истории покупок"));
                this.SaveChanges();
            }
        }

        private Town[] ReadTownsFromFile()
        {
            Random random = new Random();

            using StreamReader reader = new StreamReader(@"..\Cataloguer.Database\Resources\goroda.txt");
            return reader.ReadToEnd()
                .Split()
                .Where(x => !string.IsNullOrEmpty(x) && !x.Contains("Оспаривается"))
                .Select(x => new Town() { Name = x, Population = random.Next(5000, 20000000) })
                .ToArray();
        }

        private Good[] ReadGoodsFromFile()
        {
            Random random = new Random();

            using StreamReader reader = new StreamReader(@"..\Cataloguer.Database\Resources\goods.txt");
            return reader.ReadToEnd()
                .Split()
                .Where(x => !string.IsNullOrEmpty(x))
                .Distinct()
                .Select(x => new Good() { Name = x })
                .ToArray();
        }

        private SellHistory[] GenerateSellHistory()
        {
            const int size = 1000;

            Random random = new Random();

            var result = new SellHistory[size];

            var availableGoods = Goods.ToArray();
            var availableTowns = Towns.ToArray();
            var today = DateTime.Now.ToOADate();

            for (int i = 0; i < size; i++)
            {
                result[i] = new SellHistory()
                {
                    Good = availableGoods[random.Next(0, availableGoods.Length)],
                    Town = availableTowns[random.Next(0, availableTowns.Length)],
                    Age = (short)random.Next(7, 90),
                    SellDate = DateTime.FromOADate(today - random.NextDouble() * 100),
                    GoodCount = random.Next(1, 20)
                };
                result[i].Price = SellHistory
                    .FirstOrDefault(x => x.Good == result[i].Good)?.Price ?? 200 + random.Next(-100, 100);
            }

            return result;
        }

        private T DoWithNotification<T>(Func<T> func, string funcName)
        {
            Console.WriteLine($"Начало выполнения метода: {func.Method.Name}");

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var res = func();

            stopwatch.Stop();

            Console.WriteLine($"Выполнен метод: {funcName} за {stopwatch.Elapsed.TotalSeconds} секунд");

            return res;
        }
    }
}
