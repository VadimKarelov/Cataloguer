using Cataloguer.Database.Models;
using Cataloguer.Database.Repositories;
using System.Text.Json;

namespace Cataloguer.Server
{
    public static class DataBaseHandler
    {
        public static string GetCollection(string typename)
        {
            switch (typename)
            {
                case nameof(AgeGroup) : return Serialize(new AgeGroupRepository().TryGetAll());
                case nameof(Brochure) : return Serialize(new BrochureRepository().TryGetAll());
                case nameof(BrochurePosition) : return Serialize(new BrochurePositionRepository().TryGetAll());
                case nameof(Distribution) : return Serialize(new DistributionRepository().TryGetAll());
                case nameof(Gender) : return Serialize(new GenderRepository().TryGetAll());
                case nameof(Good) : return Serialize(new GoodRepository().TryGetAll());
                case nameof(SellHistory) : return Serialize(new SellHistoryRepository().TryGetAll());
                case nameof(Status) : return Serialize(new StatusRepository().TryGetAll());
                case nameof(Town) : return Serialize(new TownRepository().TryGetAll());
                default: Console.Error.WriteLine($"GetCollection - неверный тип! ({typename})"); return "";
            }
        }

        private static string Serialize<T>(IEnumerable<T>? objects)
        {
            return JsonSerializer.Serialize(objects);
        }
    }
}
