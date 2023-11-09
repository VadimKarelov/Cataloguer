using Cataloguer.Database.Base;
using Cataloguer.Database.Commands.GetCommands;
using Cataloguer.Server.ContextHandlers;
using Microsoft.AspNetCore.Cors;
using Serilog;
using Serilog.Formatting.Compact;

namespace Cataloguer.Server
{
    public class Program
    {
        private static string _baseRoute = string.Empty;

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
                    .UseKestrel(options =>
                    {
                        options.AllowSynchronousIO = true;
                    });

                var app = builder.Build();

                app.UseCorsMiddleware();

                DataBaseConfiguration dbConfig = new DataBaseConfiguration()
                {
                    ConnectionsString = builder.Configuration["DataBaseConnectionString"]
                };

                DefaultRoutesRegistration(app);
                ListWithoutParametersRegistration(app, dbConfig);
                GetSingleObjectRegistration(app, dbConfig);
                GetSpecialRegistration(app, dbConfig);
                AddRegistration(app, dbConfig);
                UpdateRegistration(app, dbConfig);

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

        [EnableCors()]
        private static void DefaultRoutesRegistration(WebApplication app)
        {
            app.MapGet("/", (IEnumerable<EndpointDataSource> endpointSources) =>
                string.Join("\n", endpointSources.SelectMany(x => x.Endpoints)));
        }

        [EnableCors()]
        private static void ListWithoutParametersRegistration(WebApplication app, DataBaseConfiguration config)
        {
            app.MapGet(_baseRoute + "/getAgeGroups", () => new GetCommand(config).GetListAgeGroup());
            app.MapGet(_baseRoute + "/getBrochures", () => new GetCommand(config).GetListBrochure());
            app.MapGet(_baseRoute + "/getBrochurePositions", () => new GetCommand(config).GetListBrochurePositions());
            app.MapGet(_baseRoute + "/getDistributions", () => new GetCommand(config).GetListDistribution());
            app.MapGet(_baseRoute + "/getGenders", () => new GetCommand(config).GetListGender());
            app.MapGet(_baseRoute + "/getGoods", () => new GetSpecialRequestCommand(config).GetGoodsWithAveragePriceFromHistory());
            app.MapGet(_baseRoute + "/getSellHistory", () => new GetCommand(config).GetListSellHistory());
            app.MapGet(_baseRoute + "/getStatuses", () => new GetCommand(config).GetListStatus());
            app.MapGet(_baseRoute + "/getTowns", () => new GetCommand(config).GetListTown());
        }

        [EnableCors()]
        private static void GetSingleObjectRegistration(WebApplication app, DataBaseConfiguration config)
        {
            app.MapGet(_baseRoute + "/getAgeGroup/id={id}", (int id) => new GetCommand(config).GetAgeGroup(id));
            app.MapGet(_baseRoute + "/getBrochure/id={id}", (int id) => new GetCommand(config).GetBrochure(id));
            app.MapGet(_baseRoute + "/getBrochurePosition/id={id}", (int id) => new GetCommand(config).GetBrochurePosition(id));
            app.MapGet(_baseRoute + "/getDistribution/id={id}", (int id) => new GetCommand(config).GetDistribution(id));
            app.MapGet(_baseRoute + "/getGender/id={id}", (int id) => new GetCommand(config).GetGender(id));
            app.MapGet(_baseRoute + "/getGood/id={id}", (int id) => new GetCommand(config).GetGood(id));
            app.MapGet(_baseRoute + "/getSellHistory/id={id}", (int id) => new GetCommand(config).GetSellHistory(id));
            app.MapGet(_baseRoute + "/getStatus/id={id}", (int id) => new GetCommand(config).GetStatus(id));
            app.MapGet(_baseRoute + "/getTown/id={id}", (int id) => new GetCommand(config).GetTown(id));
        }

        [EnableCors()]
        private static void GetSpecialRegistration(WebApplication app, DataBaseConfiguration config)
        {
            app.MapGet(_baseRoute + "/getBrochureGoods/id={brochureId}",
                (int brochureId) => new GetSpecialRequestCommand(config).GetGoodsFromBrochure(brochureId));

            app.MapGet(_baseRoute + "/getBrochureDistributions/id={brochureId}",
                (int brochureId) => new GetSpecialRequestCommand(config).GetDistributionsFromBrochure(brochureId));
        }

        [EnableCors()]
        private static void AddRegistration(WebApplication app, DataBaseConfiguration config)
        {
            app.MapPost(_baseRoute + "/createBrochure", (HttpContext context) => ContextHandler.AddBrochure(context, config));
            app.MapPost(_baseRoute + "/createDistribution", (HttpContext context) => ContextHandler.AddDistribution(context, config));

            app.MapPost(_baseRoute + "/addGoodsToBrochure/id={brochureId}", (HttpContext context, int brochureId) => ContextHandler.AddGoodsToBrochure(context, config, brochureId));
        }

        [EnableCors()]
        private static void UpdateRegistration(WebApplication app, DataBaseConfiguration config)
        {
            app.MapPost(_baseRoute + "/updateBrochure", (HttpContext context) => ContextHandler.UpdateBrochure(context, config));
        }
    }
}