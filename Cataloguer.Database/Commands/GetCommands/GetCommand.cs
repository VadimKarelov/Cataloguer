using Cataloguer.Database.Base;
using Cataloguer.Database.Commands.Base;
using Cataloguer.Database.Models;

namespace Cataloguer.Database.Commands.GetCommands
{
    public class GetCommand<T> : AbstractCommand where T : class
    {
        public GetCommand(DataBaseConfiguration configuration) : base(configuration) { }

        public T? GetValueById(int id)
        {
            var typename = typeof(T).ToString();

            switch (typename.Remove(0, typename.LastIndexOf(".") + 1))
            {
                case nameof(AgeGroup): return (Context.AgeGroups.FirstOrDefault(x => x.Id == id) as T);
                case nameof(Brochure): return (Context.Brochures.FirstOrDefault(x => x.Id == id) as T);
                case nameof(BrochurePosition): return (Context.BrochurePositions.FirstOrDefault(x => x.Id == id) as T);
                case nameof(Distribution): return (Context.Distributions.FirstOrDefault(x => x.Id == id) as T);
                case nameof(Gender): return (Context.Genders.FirstOrDefault(x => x.Id == id) as T);
                case nameof(Good): return (Context.Goods.FirstOrDefault(x => x.Id == id) as T);
                case nameof(SellHistory): return (Context.SellHistory.FirstOrDefault(x => x.Id == id) as T);
                case nameof(Status): return (Context.Statuses.FirstOrDefault(x => x.Id == id) as T);
                case nameof(Town): return (Context.Towns.FirstOrDefault(x => x.Id == id) as T);
                default: throw new ArgumentException($"Указанного типа ({typename}) не существует в базе данных!");
            }
        }
    }
}
