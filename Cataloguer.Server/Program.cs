using Cataloguer.Database.Base;
using Cataloguer.Database.Commands.AddOrUpdateCommand;
using Cataloguer.Database.Commands.GetCommands;
using Cataloguer.Database.Models;
using Cataloguer.Server.ContextHandlers;
using Microsoft.AspNetCore.Cors;
using Serilog;
using Serilog.Formatting.Json;

namespace Cataloguer.Server
{
    public class Program
    {
        private static string _baseRoute = string.Empty;

        public static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.File(new JsonFormatter(), "Logs/Cataloguer.log")
                .MinimumLevel.Debug()
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

                builder.WebHost.UseUrls(builder.Configuration["BackendConnectionString"]);

                builder.Host.UseSerilog();

                var app = builder.Build();

                app.UseCorsMiddleware()
                    .UseSerilogRequestLogging();

                DataBaseConfiguration dbConfig = new DataBaseConfiguration()
                {
                    ConnectionsString = builder.Configuration["DataBaseConnectionString"]
                };

                DefaultRoutesRegistration(app);
                ListWithoutParametersRegistration(app, dbConfig);
                GetSingleObjectRegistration(app, dbConfig);
                GetSpecialRegistration(app, dbConfig);
                AddRegistration(app, dbConfig);

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
            app.MapGet(_baseRoute + "/getAgeGroups", () => new GetListCommand<AgeGroup>(config).GetValues());
            app.MapGet(_baseRoute + "/getBrochures", () => new GetListCommand<Brochure>(config).GetValues());
            app.MapGet(_baseRoute + "/getBrochurePositions", () => new GetListCommand<BrochurePosition>(config).GetValues());
            app.MapGet(_baseRoute + "/getDistributions", () => new GetListCommand<Distribution>(config).GetValues());
            app.MapGet(_baseRoute + "/getGenders", () => new GetListCommand<Gender>(config).GetValues());
            app.MapGet(_baseRoute + "/getGoods", () => new GetSpecialRequestCommand(config).GetGoodsWithAveragePriceFromHistory());
            app.MapGet(_baseRoute + "/getSellHistory", () => new GetListCommand<SellHistory>(config).GetValues());
            app.MapGet(_baseRoute + "/getStatuses", () => new GetListCommand<Status>(config).GetValues());
            app.MapGet(_baseRoute + "/getTowns", () => new GetListCommand<Town>(config).GetValues());
        }

        [EnableCors()]
        private static void GetSingleObjectRegistration(WebApplication app, DataBaseConfiguration config)
        {
            app.Map(_baseRoute + "/getAgeGroup/id={id}", (int id) => new GetCommand<AgeGroup>(config).GetValueById(id));
            app.Map(_baseRoute + "/getBrochure/id={id}", (int id) => new GetCommand<Brochure>(config).GetValueById(id));
            app.Map(_baseRoute + "/getBrochurePosition/id={id}", (int id) => new GetCommand<BrochurePosition>(config).GetValueById(id));
            app.Map(_baseRoute + "/getDistribution/id={id}", (int id) => new GetCommand<Distribution>(config).GetValueById(id));
            app.Map(_baseRoute + "/getGender/id={id}", (int id) => new GetCommand<Gender>(config).GetValueById(id));
            app.Map(_baseRoute + "/getGood/id={id}", (int id) => new GetCommand<Good>(config).GetValueById(id));
            app.Map(_baseRoute + "/getSellHistory/id={id}", (int id) => new GetCommand<SellHistory>(config).GetValueById(id));
            app.Map(_baseRoute + "/getStatus/id={id}", (int id) => new GetCommand<Status>(config).GetValueById(id));
            app.Map(_baseRoute + "/getTown/id={id}", (int id) => new GetCommand<Town>(config).GetValueById(id));
        }

        [EnableCors()]
        private static void GetSpecialRegistration(WebApplication app, DataBaseConfiguration config)
        {
            app.Map(_baseRoute + "/getBrochureGoods/id={brochureId}",
                (int brochureId) => new GetSpecialRequestCommand(config).GetGoodsFromBrochure(brochureId));

            app.Map(_baseRoute + "/getBrochureDistributions/id={brochureId}",
                (int brochureId) => new GetSpecialRequestCommand(config).GetDistributionsFromBrochure(brochureId));
        }

        [EnableCors()]
        private static void AddRegistration(WebApplication app, DataBaseConfiguration config)
        {
            app.Map(_baseRoute + "/addBrochure", (HttpContext context) => ContextHandler.AddBrochure(context, config));
        }
    }
}