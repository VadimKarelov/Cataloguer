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

            app.MapGet("/", () => "Hello World!");

            BrochureHandlerRegistration(app);

            app.Run();
        }

        private static void BrochureHandlerRegistration(WebApplication app)
        {
            app.MapGet($"{_baseRoute}/getBrochures", 
                () => JsonSerializer.Serialize(new GetListCommand<Brochure>().GetValues()));

            app.MapGet($"{_baseRoute}/getAgeGroups",
                () => JsonSerializer.Serialize(new GetListCommand<AgeGroup>().GetValues()));
        }
    }
}