using Cataloguer.Database.Commands.GetCommands;
using Cataloguer.Database.Models;
using System.Text.Json;

namespace Cataloguer.Server
{
    public class Program
    {
        private const string _baseRoute = "/api/v1/cataloguer";

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

            app.MapGet("/", (IEnumerable<EndpointDataSource> endpointSources) => 
                string.Join("\n", endpointSources.SelectMany(x => x.Endpoints)));

            ListWithoutParametersRegistration(app);
            GetSingleObjectRegistration(app);

            app.Run();
        }

        private static void AddRoute(WebApplication app, string route, Func<object> action)
        {
            app.MapGet($"{_baseRoute}/{route}", () => JsonSerializer.Serialize(action()));
        }

        private static void ListWithoutParametersRegistration(WebApplication app)
        {
            // в идеале прописать пути: get/AgeGroup или get/Town, то есть указать сущность. но сойдет и так
            AddRoute(app, "getAgeGroups", () => new GetListCommand<AgeGroup>().GetValues());
            AddRoute(app, "getBrochures", () => new GetListCommand<Brochure>().GetValues());
            AddRoute(app, "getBrochurePositions", () => new GetListCommand<BrochurePosition>().GetValues());
            AddRoute(app, "getDistributions", () => new GetListCommand<Distribution>().GetValues());
            AddRoute(app, "getGenders", () => new GetListCommand<Gender>().GetValues());
            AddRoute(app, "getGoods", () => new GetListCommand<Good>().GetValues());
            AddRoute(app, "getSellHistory", () => new GetListCommand<SellHistory>().GetValues());
            AddRoute(app, "getStatuses", () => new GetListCommand<Status>().GetValues());
            AddRoute(app, "getTowns", () => new GetListCommand<Town>().GetValues());
        }

        private static void GetSingleObjectRegistration(WebApplication app)
        {
            app.Map("/getBrochure/id={id}", (int id) => JsonSerializer.Serialize(new GetCommand<Brochure>().GetValueById(id)));
            app.Map("/getGood/id={id}", (int id) => JsonSerializer.Serialize(new GetCommand<Good>().GetValueById(id)));
        }
    }
}