﻿using Cataloguer.Database.Commands.GetCommands;
using Cataloguer.Database.Models;
using System.Text.Json;

namespace Cataloguer.Server
{
    public static class RoutesRegistrationExtension
    {
        private const string _baseRoute = "/api/v1/cataloguer";

        public static WebApplication RegisterRoutes(this WebApplication app)
        {
            return app.DefaultRoutesRegistration()
                .ListWithoutParametersRegistration()
                .GetSingleObjectRegistration();
        }

        private static WebApplication DefaultRoutesRegistration(this WebApplication app)
        {
            app.MapGet("/", (IEnumerable<EndpointDataSource> endpointSources) =>
                string.Join("\n", endpointSources.SelectMany(x => x.Endpoints)));

            return app;
        }

        private static WebApplication ListWithoutParametersRegistration(this WebApplication app)
        {
            // в идеале прописать пути: get/AgeGroup или get/Town, то есть указать сущность. но сойдет и так
            app.MapGet(_baseRoute + "getAgeGroups", () => JsonSerializer.Serialize(() => new GetListCommand<AgeGroup>().GetValues()));
            app.MapGet(_baseRoute + "getBrochures", () => JsonSerializer.Serialize(new GetListCommand<Brochure>().GetValues()));
            app.MapGet(_baseRoute + "getBrochurePositions", () => JsonSerializer.Serialize(new GetListCommand<BrochurePosition>().GetValues()));
            app.MapGet(_baseRoute + "getDistributions", () => JsonSerializer.Serialize(new GetListCommand<Distribution>().GetValues()));
            app.MapGet(_baseRoute + "getGenders", () => JsonSerializer.Serialize(new GetListCommand<Gender>().GetValues()));
            app.MapGet(_baseRoute + "getGoods", () => JsonSerializer.Serialize(new GetListCommand<Good>().GetValues()));
            app.MapGet(_baseRoute + "getSellHistory", () => JsonSerializer.Serialize(new GetListCommand<SellHistory>().GetValues()));
            app.MapGet(_baseRoute + "getStatuses", () => JsonSerializer.Serialize(new GetListCommand<Status>().GetValues()));
            app.MapGet(_baseRoute + "getTowns", () => JsonSerializer.Serialize(new GetListCommand<Town>().GetValues()));

            return app;
        }

        private static WebApplication GetSingleObjectRegistration(this WebApplication app)
        {
            app.Map(_baseRoute + "/getAgeGroup/id={id}", (int id) => JsonSerializer.Serialize(new GetCommand<AgeGroup>().GetValueById(id)));
            app.Map(_baseRoute + "/getBrochure/id={id}", (int id) => JsonSerializer.Serialize(new GetCommand<Brochure>().GetValueById(id)));
            app.Map(_baseRoute + "/getBrochurePosition/id={id}", (int id) => JsonSerializer.Serialize(new GetCommand<BrochurePosition>().GetValueById(id)));
            app.Map(_baseRoute + "/getDistribution/id={id}", (int id) => JsonSerializer.Serialize(new GetCommand<Distribution>().GetValueById(id)));
            app.Map(_baseRoute + "/getGender/id={id}", (int id) => JsonSerializer.Serialize(new GetCommand<Gender>().GetValueById(id)));
            app.Map(_baseRoute + "/getGood/id={id}", (int id) => JsonSerializer.Serialize(new GetCommand<Good>().GetValueById(id)));
            app.Map(_baseRoute + "/getSellHistory/id={id}", (int id) => JsonSerializer.Serialize(new GetCommand<SellHistory>().GetValueById(id)));
            app.Map(_baseRoute + "/getStatus/id={id}", (int id) => JsonSerializer.Serialize(new GetCommand<Status>().GetValueById(id)));
            app.Map(_baseRoute + "/getTown/id={id}", (int id) => JsonSerializer.Serialize(new GetCommand<Town>().GetValueById(id)));

            return app;
        }
    }
}
