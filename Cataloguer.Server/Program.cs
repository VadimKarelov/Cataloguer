using Cataloguer.Database.Models;

namespace Cataloguer.Server
{
    public class Program
    {
        private const string _baseRoute = "/api/v1/cataloguer";

        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var app = builder.Build();

            app.MapGet("/", () => "Hello World!");

            BrochureHandlerRegistration(app);

            app.Run();
        }

        private static void BrochureHandlerRegistration(WebApplication app)
        {
            app.MapGet($"{_baseRoute}/getBrochures", 
                () => DataBaseHandler.GetCollection(nameof(Brochure)));

            app.MapGet($"{_baseRoute}/getAgeGroups",
                () => DataBaseHandler.GetCollection(nameof(AgeGroup)));
        }
    }
}