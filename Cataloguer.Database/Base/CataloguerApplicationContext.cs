using System.Diagnostics;
using Cataloguer.Common.Models;
using Cataloguer.Common.Models.SpecialModels.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Cataloguer.Database.Base;

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
    public DbSet<PredictedSellHistory> PredictedSellHistory { get; set; }
    public DbSet<SellHistory> SellHistory { get; set; }
    public DbSet<Status> Statuses { get; set; }
    public DbSet<Town> Towns { get; set; }

    public DbSet<LogEntity> Logs { get; set; }

    private static bool _isInitialised;
    private static readonly int _sellHistoryCount = 1000;

    private readonly string _connectionString;

    public CataloguerApplicationContext(DataBaseConfiguration config)
    {
        _connectionString = config.ConnectionsString;

        if (!_isInitialised)
        {
            Database.EnsureCreated();
        }
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(_connectionString);
        optionsBuilder.LogTo(Log.Logger.Error, LogLevel.Error);
        optionsBuilder.LogTo(Log.Logger.Fatal, LogLevel.Critical);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AgeGroup>().HasData(
            new AgeGroup() { Id = 1, Description = "Дети", MinimalAge = 0, MaximalAge = 14 },
            new AgeGroup { Id = 2, Description = "Подростки", MinimalAge = 15, MaximalAge = 20 },
            new AgeGroup { Id = 3, Description = "Молодые", MinimalAge = 21, MaximalAge = 35 },
            new AgeGroup { Id = 4, Description = "Взрослые", MinimalAge = 36, MaximalAge = 50 },
            new AgeGroup { Id = 5, Description = "Пожилые", MinimalAge = 51, MaximalAge = short.MaxValue });

        modelBuilder.Entity<Gender>().HasData(new Gender() { Id = 1, Name = "Мужской" },
            new Gender { Id = 2, Name = "Женский" }, new Gender { Id = 3, Name = "Боевой вертолет" });

        modelBuilder.Entity<Status>().HasData(
            new Status() { Id = 1, Name = "Не выпущен" },
            new Status { Id = 2, Name = "Выпущен" });

        modelBuilder.Entity<SellHistory>().HasKey(x => x.Id);
    }

    /// <summary>
    /// Обязательно вызвать после создания Context
    /// </summary>
    public void Init()
    {
        if (!_isInitialised)
        {
            var random = new Random();

            {
                var fromFile = ReadTownsFromFile();

                if (fromFile.Any())
                {
                    var toRemove = Towns.AsNoTracking()
                        .Where(x => !fromFile.Contains(x.Name));

                    var toAdd = fromFile.Except(Towns.Select(x => x.Name))
                        .Select(x => new Town
                        {
                            Name = x,
                            Population = random.Next(5000, 20000000)
                        });

                    Towns.RemoveRange(toRemove);
                    Towns.AddRange(toAdd);
                }
            }

            {
                var fromFile = DoWithNotification(ReadGoodsFromFile, "Чтение товаров");

                if (fromFile.Any())
                {
                    var toRemove = Goods.AsNoTracking()
                        .Where(x => !fromFile.Contains(x.Name));

                    var toAdd = fromFile.Except(Goods.Select(x => x.Name))
                        .Select(x => new Good { Name = x, Price = random.Next(1000, 10000)});

                    Goods.RemoveRange(toRemove);
                    Goods.AddRange(toAdd);
                }
            }

            SaveChanges();

            SellHistory.AddRange(DoWithNotification(
                () => GenerateSellHistory(Math.Max(0, _sellHistoryCount - SellHistory.Count())),
                "Создание истории покупок"));

            SaveChanges();

            _isInitialised = true;
        }
    }

    private string[] ReadTownsFromFile()
    {
        // при запуске из VS и exe файла разные пути до файлов
        string[] paths = { @"Resources\goroda.txt", @"..\Cataloguer.Database\Resources\goroda.txt" };

        foreach (var path in paths)
            try
            {
                using var reader = new StreamReader(path);
                return reader.ReadToEnd()
                    .Split('\n')
                    .Where(x => !string.IsNullOrEmpty(x) && !x.Contains("Оспаривается"))
                    .Select(x => x.Replace("\r", ""))
                    .Where(x => !string.IsNullOrEmpty(x))
                    .Distinct()
                    .ToArray();
            }
            catch
            {
            }

        Log.Error("Не удалось считать файл с городами!");

        return Array.Empty<string>();
    }

    private string[] ReadGoodsFromFile()
    {
        // при запуске из VS и exe файла разные пути до файлов
        string[] paths = { @"Resources\goods.txt", @"..\Cataloguer.Database\Resources\goods.txt" };

        foreach (var path in paths)
            try
            {
                using var reader = new StreamReader(path);
                return reader.ReadToEnd()
                    .Split('\n')
                    .Where(x => !string.IsNullOrEmpty(x))
                    .Distinct()
                    .Select(x => x.Replace("\r", ""))
                    .Where(x => !string.IsNullOrEmpty(x))
                    .Distinct()
                    .ToArray();
            }
            catch
            {
            }

        Log.Error("Не удалось считать файл с продуктами!");

        return Array.Empty<string>();
    }

    private List<SellHistory> GenerateSellHistory(int count)
    {
        var random = new Random();

        var result = new List<SellHistory>();

        var availableGoods = Goods.ToArray();
        var availableTowns = Towns.ToArray();
        var availableGenders = Genders.ToArray();
        var today = DateTime.Now.ToOADate();

        for (var i = count; i > 0; i--)
        {
            var entity = new SellHistory
            {
                Good = availableGoods[random.Next(0, availableGoods.Length)],
                Town = availableTowns[random.Next(0, availableTowns.Length)],
                Age = (short)random.Next(7, 90),
                SellDate = DateTime.FromOADate(today - random.NextDouble() * 100),
                GoodCount = random.Next(1, 20),
                Gender = availableGenders[random.Next(0, availableGenders.Length)]
            };
            entity.Price = SellHistory
                .FirstOrDefault(x => x.Good == entity.Good)?.Price ?? 200 + random.Next(-100, 100);

            result.Add(entity);
        }

        return result;
    }

    private T DoWithNotification<T>(Func<T> func, string funcName)
    {
        Log.Debug($"Начало выполнения метода: {funcName}");

        var stopwatch = new Stopwatch();
        stopwatch.Start();

        var res = func();

        stopwatch.Stop();

        Log.Debug($"Выполнен метод: {funcName} за {stopwatch.Elapsed.TotalSeconds} секунд");

        return res;
    }
}