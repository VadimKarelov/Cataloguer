using Cataloguer.Database.Base;
using Cataloguer.Database.Commands.Base;
using Cataloguer.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Cataloguer.Database.Commands.GetCommands
{
    public class GetListCommand<T> : AbstractCommand where T : class
    {
        public GetListCommand(DataBaseConfiguration config) : base(config) { }

        public IEnumerable<T> GetValues()
        {
            var typename = typeof(T).ToString();

            switch (typename.Remove(0, typename.LastIndexOf(".") + 1))
            {
                case nameof(AgeGroup): return Context.AgeGroups.AsNoTracking().Cast<T>().ToArray();
                case nameof(Brochure): return Context.Brochures.AsNoTracking().Cast<T>().ToArray();
                case nameof(BrochurePosition): return Context.BrochurePositions.AsNoTracking().Cast<T>().ToArray();
                case nameof(Distribution): return Context.Distributions.AsNoTracking().Cast<T>().ToArray();
                case nameof(Gender): return Context.Genders.AsNoTracking().Cast<T>().ToArray();
                case nameof(Good): return Context.Goods.AsNoTracking().Cast<T>().ToArray();
                case nameof(SellHistory): return Context.SellHistory.AsNoTracking().Cast<T>().ToArray();
                case nameof(Status): return Context.Statuses.AsNoTracking().Cast<T>().ToArray();
                case nameof(Town): return Context.Towns.AsNoTracking().Cast<T>().ToArray();
                default: throw new ArgumentException($"Указанного типа ({typename}) не существует в базе данных!");
            }
        }

        public IEnumerable<T> GetValues(Func<T, bool> predicate)
        {
            var typename = typeof(T).ToString();

            switch (typename.Remove(0, typename.LastIndexOf(".") + 1))
            {
                case nameof(AgeGroup): return Context.AgeGroups.AsNoTracking().Where(x => predicate(x as T)).Cast<T>().ToArray();
                case nameof(Brochure): return Context.Brochures.AsNoTracking().Where(x => predicate(x as T)).Cast<T>().ToArray();
                case nameof(BrochurePosition): return Context.BrochurePositions.AsNoTracking().Where(x => predicate(x as T)).Cast<T>().ToArray();
                case nameof(Distribution): return Context.Distributions.AsNoTracking().Where(x => predicate(x as T)).Cast<T>().ToArray();
                case nameof(Gender): return Context.Genders.AsNoTracking().Where(x => predicate(x as T)).Cast<T>().ToArray();
                case nameof(Good): return Context.Goods.AsNoTracking().Where(x => predicate(x as T)).Cast<T>().ToArray();
                case nameof(SellHistory): return Context.SellHistory.AsNoTracking().Where(x => predicate(x as T)).Cast<T>().ToArray();
                case nameof(Status): return Context.Statuses.AsNoTracking().Where(x => predicate(x as T)).Cast<T>().ToArray();
                case nameof(Town): return Context.Towns.AsNoTracking().Where(x => predicate(x as T)).Cast<T>().ToArray();
                default: throw new ArgumentException($"Указанного типа ({typename}) не существует в базе данных!");
            }
        }
    }
}
