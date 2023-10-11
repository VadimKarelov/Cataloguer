using Cataloguer.Database.Models;
using Microsoft.AspNetCore.Cors;

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
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });

            var app = builder.Build();
                        
            app.MapGet("/", () => "Hello World!");
            app.UseCorsMiddleware();
            BrochureHandlerRegistration(app);

            app.Run();
        }

        [EnableCors()]
        private static void BrochureHandlerRegistration(WebApplication app)
        {
            app.MapGet($"{_baseRoute}/getBrochures", 
                () => DataBaseHandler.GetCollection(nameof(Brochure)));

            app.MapGet($"{_baseRoute}/getAgeGroups",
                () => DataBaseHandler.GetCollection(nameof(AgeGroup)));
        }
    }
}