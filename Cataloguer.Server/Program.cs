using Cataloguer.Database.Base;
using Cataloguer.Database.Commands;
using Cataloguer.Database.Commands.AddOrUpdateCommands;
using Cataloguer.Database.Commands.GetCommands;
using Cataloguer.Server.ContextHandlers;
using Cataloguer.Server.Modules;
using Microsoft.AspNetCore.Cors;
using Serilog;
using Serilog.Formatting.Compact;

namespace Cataloguer.Server;

public class Program
{
    private static string _baseRoute = string.Empty;

    /// <summary>
    /// Рекурсивно перемещает все вложенныые файлы и каталоги.
    /// </summary>
    /// <param name="sourceDirectory">Каталог, из которого надо перемещать</param>
    /// <param name="targetDirectory">Каталог, в который надо перемещать</param>
    private static void MoveStaticFiles(string sourceDirectory, string targetDirectory)
    {
        try
        {
            // Если каталог назначения не существует, создаем его
            if (!Directory.Exists(targetDirectory))
            {
                Directory.CreateDirectory(targetDirectory);
            }

            // Перемещаем все файлы
            foreach (string file in Directory.GetFiles(sourceDirectory))
            {
                string fileName = Path.GetFileName(file);
                string destFile = Path.Combine(targetDirectory, fileName);
                File.Move(file, destFile);
            }

            // Рекурсивно перемещаем все подкаталоги с их файлами и каталогами
            foreach (string subdirectory in Directory.GetDirectories(sourceDirectory))
            {
                string subdirectoryName = Path.GetFileName(subdirectory);
                string destSubdirectory = Path.Combine(targetDirectory, subdirectoryName);
                MoveStaticFiles(subdirectory, destSubdirectory);
            }

            // После перемещения всех файлов и каталогов, удаляем исходный каталог
            Directory.Delete(sourceDirectory);
        }
        catch (Exception e)
        {
            Console.WriteLine("Ошибка при перемещении файлов и каталогов: " + e.Message);
        }
    }
    
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.File(new CompactJsonFormatter(), "Logs/Cataloguer.log")
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .CreateLogger();

        try
        {
            var builder = WebApplication.CreateBuilder(args);

            var currentPath = Directory.GetCurrentDirectory();
            var parentPath = Directory.GetParent(currentPath)?.FullName;
            if (parentPath != null)
            {
                var sourceDirectory = Path.Combine(parentPath, "frontend-app", "build");
                var destinationDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
                MoveStaticFiles(sourceDirectory, destinationDirectory);
            }
            
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });

            builder.Configuration.AddJsonFile("appsettings.json");

            _baseRoute = builder.Configuration["BaseRoute"];

            builder.WebHost.UseUrls(builder.Configuration["BackendConnectionString"])
                .UseKestrel(options => { options.AllowSynchronousIO = true; });

            builder.Host.UseSerilog();

            var app = builder.Build();

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseCorsMiddleware();

            var dbConfig = new DataBaseConfiguration
            {
                ConnectionsString = builder.Configuration["DataBaseConnectionString"]
            };

            DefaultRoutesRegistration(app);
            ListWithoutParametersRegistration(app, dbConfig);
            GetSingleObjectRegistration(app, dbConfig);
            GetSpecialRegistration(app, dbConfig);
            AddRegistration(app, dbConfig);
            UpdateRegistration(app, dbConfig);
            DeleteRegistration(app, dbConfig);
            RunBackgroundProcessRegistration(app, dbConfig);

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "boom");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    [EnableCors]
    private static void DefaultRoutesRegistration(WebApplication app)
    {
        app.MapGet(_baseRoute, (IEnumerable<EndpointDataSource> endpointSources) =>
            string.Join("\n", endpointSources.SelectMany(x => x.Endpoints)));
    }

    [EnableCors]
    private static void ListWithoutParametersRegistration(WebApplication app, DataBaseConfiguration config)
    {
        app.MapGet(_baseRoute + "/getAgeGroups", () => new GetCommand(config).GetListAgeGroup());
        app.MapGet(_baseRoute + "/getBrochures", () => new GetCommand(config).GetListBrochure());
        app.MapGet(_baseRoute + "/getBrochurePositions", () => new GetCommand(config).GetListBrochurePositions());
        app.MapGet(_baseRoute + "/getDistributions", () => new GetCommand(config).GetListDistribution());
        app.MapGet(_baseRoute + "/getGenders", () => new GetCommand(config).GetListGender());
        app.MapGet(_baseRoute + "/getGoods", () => new GetCommand(config).GetListGood());
        app.MapGet(_baseRoute + "/getSellHistory", () => new GetCommand(config).GetListSellHistory());
        app.MapGet(_baseRoute + "/getStatuses", () => new GetCommand(config).GetListStatus());
        app.MapGet(_baseRoute + "/getTowns", () => new GetCommand(config).GetListTown());

        app.MapGet(_baseRoute + "/getLogs", () => new GetCommand(config).GetListLog());
    }

    [EnableCors]
    private static void GetSingleObjectRegistration(WebApplication app, DataBaseConfiguration config)
    {
        app.MapGet(_baseRoute + "/getAgeGroup/id={id}", (int id) => new GetCommand(config).GetAgeGroup(id));
        app.MapGet(_baseRoute + "/getBrochure/id={id}", (int id) => new GetCommand(config).GetBrochure(id));
        app.MapGet(_baseRoute + "/getBrochurePosition/id={id}",
            (int id) => new GetCommand(config).GetBrochurePosition(id));
        app.MapGet(_baseRoute + "/getDistribution/id={id}", (int id) => new GetCommand(config).GetDistribution(id));
        app.MapGet(_baseRoute + "/getGender/id={id}", (int id) => new GetCommand(config).GetGender(id));
        app.MapGet(_baseRoute + "/getGood/id={id}", (int id) => new GetCommand(config).GetGood(id));
        app.MapGet(_baseRoute + "/getSellHistory/id={id}", (int id) => new GetCommand(config).GetSellHistory(id));
        app.MapGet(_baseRoute + "/getStatus/id={id}", (int id) => new GetCommand(config).GetStatus(id));
        app.MapGet(_baseRoute + "/getTown/id={id}", (int id) => new GetCommand(config).GetTown(id));
    }

    [EnableCors]
    private static void GetSpecialRegistration(WebApplication app, DataBaseConfiguration config)
    {
        app.MapGet(_baseRoute + "/getBrochureGoods/id={brochureId}",
            (int brochureId) => new GetSpecialRequestCommand(config).GetGoodsFromBrochure(brochureId));

        app.MapGet(_baseRoute + "/getBrochureDistributions/id={brochureId}",
            (int brochureId) => new GetSpecialRequestCommand(config).GetDistributionsFromBrochure(brochureId));

        app.MapGet(_baseRoute + "/getUnselectedBrochureGoods/id={brochureId}",
            (int brochureId) => new GetSpecialRequestCommand(config).GetGoodsNotFromBrochure(brochureId));

        app.MapGet(_baseRoute + "/getSellHistoryForChart/id={brochureId}",
            (int brochureId) => new GetSpecialRequestCommand(config).GetSellHistoryForChart(brochureId));

        app.MapGet(_baseRoute + "/getPredictedSellHistoryForChart/id={brochureId}",
            (int brochureId) => new GetSpecialRequestCommand(config).GetPredictedSellHistoryForChart(brochureId));
    }

    [EnableCors]
    private static void AddRegistration(WebApplication app, DataBaseConfiguration config)
    {
        app.MapPost(_baseRoute + "/createBrochure",
            (HttpContext context) => ContextHandler.AddBrochure(context, config));
        app.MapPost(_baseRoute + "/createDistribution",
            (HttpContext context) => ContextHandler.AddDistribution(context, config));

        app.MapPost(_baseRoute + "/addGoodsToBrochure/id={brochureId}",
            (HttpContext context, int brochureId) => ContextHandler.AddGoodsToBrochure(context, config, brochureId));
    }

    [EnableCors]
    private static void UpdateRegistration(WebApplication app, DataBaseConfiguration config)
    {
        app.MapPost(_baseRoute + "/updateBrochure/id={brochureId}",
            (HttpContext context, int brochureId) => ContextHandler.UpdateBrochure(context, config, brochureId));
        app.MapPost(_baseRoute + "/updateDistribution/id={distributionId}",
            (HttpContext context, int distributionId) =>
                ContextHandler.UpdateDistribution(context, config, distributionId));
        app.MapPost(_baseRoute + "/updateBrochurePosition/brochureId={brochureId}&goodId={goodId}&newPrice={newPrice}",
            (int brochureId, int goodId, decimal newPrice) =>
                new AddOrUpdateCommand(config).UpdateBrochurePosition(brochureId, goodId, newPrice));
    }

    [EnableCors]
    private static void DeleteRegistration(WebApplication app, DataBaseConfiguration config)
    {
        app.Map(_baseRoute + "/deleteBrochure/id={brochureId}",
            (int brochureId) => new DeleteCommand(config).DeleteBrochure(brochureId));
        app.Map(_baseRoute + "/deleteDistribution/id={distributionId}",
            (int distributionId) => new DeleteCommand(config).DeleteDistribution(distributionId));

        app.Map(_baseRoute + "/deleteBrochureGood/id={id}&brochureId={brochureId}",
            (int id, int brochureId) => new DeleteCommand(config).DeleteGoodFromBrochure(id, brochureId));
    }

    [EnableCors]
    private static void RunBackgroundProcessRegistration(WebApplication app, DataBaseConfiguration config)
    {
        app.MapGet(_baseRoute + "/computeBrochurePotentialIncome/id={brochureId}",
            (int brochureId) => BrochureAnalyzer.TryComputeBrochureIncome(config, brochureId));

        app.Map(_baseRoute + "/releaseBrochure/id={brochureId}",
            (int brochureId) => new SpecialAddOrUpdateCommand(config).TryMarkBrochureAsReleased(brochureId));
    }
}