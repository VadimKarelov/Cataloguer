using Cataloguer.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;
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

        private static bool _isInitialised = false;
        private static readonly int _sellHistoryCount = 10000;

        public CataloguerApplicationContext(DataBaseConfiguration config)
        {
            _connectionString = config.ConnectionsString;

            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_connectionString);
            optionsBuilder.LogTo(Log.Logger.Information, LogLevel.Information);
            optionsBuilder.LogTo(Log.Logger.Debug, LogLevel.Debug);
            optionsBuilder.LogTo(Log.Logger.Error, LogLevel.Error);
            optionsBuilder.LogTo(Log.Logger.Fatal, LogLevel.Critical);
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

        /// <summary>
        /// Обязательно вызвать после создания Context
        /// </summary>
        public void Init()
        {
            if (!_isInitialised)
            {
                {
                    var fromFile = ReadTownsFromFile();
                    var toRemove = Towns.AsEnumerable().Except(fromFile);
                    var toAdd = fromFile.AsEnumerable().Except(Towns);
                    Towns.RemoveRange(toRemove);
                    Towns.AddRange(toAdd);
                }

                {
                    var fromFile = DoWithNotification(ReadGoodsFromFile, "Чтение товаров");
                    var toRemove = Goods.AsEnumerable().Except(fromFile);
                    var toAdd = fromFile.AsEnumerable().Except(Goods);
                    Goods.RemoveRange(toRemove);
                    Goods.AddRange(toAdd);
                }

                this.SaveChanges();

                SellHistory.AddRange(DoWithNotification(() => GenerateSellHistory(Math.Max(0, _sellHistoryCount - SellHistory.Count())), "Создание истории покупок"));

                this.SaveChanges();

                _isInitialised = true;
            }
        }

        private Town[] ReadTownsFromFile()
        {
            Random random = new Random();

            using StreamReader reader = new StreamReader(@"..\Cataloguer.Database\Resources\goroda.txt");
            return reader.ReadToEnd()
                .Split('\n')
                .Where(x => !string.IsNullOrEmpty(x) && !x.Contains("Оспаривается"))
                .Select(x => x.Replace("\r", ""))
                .Select(x => new Town() { Name = x, Population = random.Next(5000, 20000000) })
                .ToArray();
        }

        private Good[] ReadGoodsFromFile()
        {
            Random random = new Random();

            using StreamReader reader = new StreamReader(@"..\Cataloguer.Database\Resources\goods.txt");
            return reader.ReadToEnd()
                .Split('\n')
                .Where(x => !string.IsNullOrEmpty(x))
                .Distinct()
                .Select(x => x.Replace("\r", ""))
                .Select(x => new Good() { Name = x })
                .ToArray();
        }

        private List<SellHistory> GenerateSellHistory(int count)
        {
            Random random = new Random();

            var result = new List<SellHistory>();

            var availableGoods = Goods.ToArray();
            var availableTowns = Towns.ToArray();
            var today = DateTime.Now.ToOADate();

            for (int i = count; i > 0; i--)
            {
                var entity = new SellHistory()
                {
                    Good = availableGoods[random.Next(0, availableGoods.Length)],
                    Town = availableTowns[random.Next(0, availableTowns.Length)],
                    Age = (short)random.Next(7, 90),
                    SellDate = DateTime.FromOADate(today - random.NextDouble() * 100),
                    GoodCount = random.Next(1, 20)
                };
                entity.Price = SellHistory
                    .FirstOrDefault(x => x.Good == entity.Good)?.Price ?? 200 + random.Next(-100, 100);

                result.Add(entity);
            }

            return result;
        }

        private T DoWithNotification<T>(Func<T> func, string funcName)
        {
            Log.Debug($"Начало выполнения метода: {func.Method.Name}");

            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();

            var res = func();

            stopwatch.Stop();

            Log.Debug($"Выполнен метод: {funcName} за {stopwatch.Elapsed.TotalSeconds} секунд");

            return res;
        }
    }
}
