using Cataloguer.Database.Commands.GetCommands;
using Cataloguer.Database.Models;
using System.Text.Json;

namespace Cataloguer.Server
{
    public class Program
    {
        private const string _baseRoute = "/api/v1/cataloguer";

        private static List<(string Route, string Description)> _existingRoutes = new();

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });

            var app = builder.Build();

            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            app.MapGet("/", ShowAllRegisteredRoutes);

            ListWithoutParametersRegistration(app);

            app.Run();
        }

        private static void AddRoute(WebApplication app, string route, string description, Func<object> action)
        {
            app.MapGet($"{_baseRoute}/{route}", () => JsonSerializer.Serialize(action()));

            _existingRoutes.Add(new($"{_baseRoute}/{route}", description));
        }

        private static string ShowAllRegisteredRoutes()
        {
            return string.Join('\n', _existingRoutes.Select(x => $"{x.Route} = {x.Description}"));
        }

        private static void ListWithoutParametersRegistration(WebApplication app)
        {
            // в идеале прописать пути: get/AgeGroup или get/Town, то есть указать сущность. но сойдет и так
            AddRoute(app, "getAgeGroups", "no params", () => new GetListCommand<AgeGroup>().GetValues());
            AddRoute(app, "getBrochures", "no params", () => new GetListCommand<Brochure>().GetValues());
            AddRoute(app, "getBrochurePositions", "no params", () => new GetListCommand<BrochurePosition>().GetValues());
            AddRoute(app, "getDistributions", "no params", () => new GetListCommand<Distribution>().GetValues());
            AddRoute(app, "getGenders", "no params", () => new GetListCommand<Gender>().GetValues());
            AddRoute(app, "getGoods", "no params", () => new GetListCommand<Good>().GetValues());
            AddRoute(app, "getSellHistory", "no params", () => new GetListCommand<SellHistory>().GetValues());
            AddRoute(app, "getStatuses", "no params", () => new GetListCommand<Status>().GetValues());
            AddRoute(app, "getTowns", "no params", () => new GetListCommand<Town>().GetValues());
        }
    }
}