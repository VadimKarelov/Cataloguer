using Cataloguer.Database.Base;
using Cataloguer.Database.Commands.Base;
using Cataloguer.Database.Models;
using Microsoft.EntityFrameworkCore;

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
                case nameof(AgeGroup): return (Context.AgeGroups.AsNoTracking().FirstOrDefault(x => x.Id == id) as T);
                case nameof(Brochure): return (Context.Brochures.AsNoTracking().FirstOrDefault(x => x.Id == id) as T);
                case nameof(BrochurePosition): return (Context.BrochurePositions.AsNoTracking().FirstOrDefault(x => x.Id == id) as T);
                case nameof(Distribution): return (Context.Distributions.FirstOrDefault(x => x.Id == id) as T);
                case nameof(Gender): return (Context.Genders.AsNoTracking().FirstOrDefault(x => x.Id == id) as T);
                case nameof(Good): return (Context.Goods.AsNoTracking().FirstOrDefault(x => x.Id == id) as T);
                case nameof(SellHistory): return (Context.SellHistory.AsNoTracking().FirstOrDefault(x => x.Id == id) as T);
                case nameof(Status): return (Context.Statuses.AsNoTracking().FirstOrDefault(x => x.Id == id) as T);
                case nameof(Town): return (Context.Towns.AsNoTracking().FirstOrDefault(x => x.Id == id) as T);
                default: throw new ArgumentException($"Указанного типа ({typename}) не существует в базе данных!");
            }
        }
    }
}
