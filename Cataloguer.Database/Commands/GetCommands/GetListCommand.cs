using Cataloguer.Database.Base;
using Cataloguer.Database.Commands.Base;
using Cataloguer.Database.Models;

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
                case nameof(AgeGroup): return Context.AgeGroups.Cast<T>().ToArray();
                case nameof(Brochure): return Context.Brochures.Cast<T>().ToArray();
                case nameof(BrochurePosition): return Context.BrochurePositions.Cast<T>().ToArray();
                case nameof(Distribution): return Context.Distributions.Cast<T>().ToArray();
                case nameof(Gender): return Context.Genders.Cast<T>().ToArray();
                case nameof(Good): return Context.Goods.Cast<T>().ToArray();
                case nameof(SellHistory): return Context.SellHistory.Cast<T>().ToArray();
                case nameof(Status): return Context.Statuses.Cast<T>().ToArray();
                case nameof(Town): return Context.Towns.Cast<T>().ToArray();
                default: throw new ArgumentException($"Указанного типа ({typename}) не существует в базе данных!");
            }
        }

        public IEnumerable<T> GetValues(Func<T, bool> predicate)
        {
            var typename = typeof(T).ToString();

            switch (typename.Remove(0, typename.LastIndexOf(".") + 1))
            {
                case nameof(AgeGroup): return Context.AgeGroups.Where(x => predicate(x as T)).Cast<T>().ToArray();
                case nameof(Brochure): return Context.Brochures.Where(x => predicate(x as T)).Cast<T>().ToArray();
                case nameof(BrochurePosition): return Context.BrochurePositions.Where(x => predicate(x as T)).Cast<T>().ToArray();
                case nameof(Distribution): return Context.Distributions.Where(x => predicate(x as T)).Cast<T>().ToArray();
                case nameof(Gender): return Context.Genders.Where(x => predicate(x as T)).Cast<T>().ToArray();
                case nameof(Good): return Context.Goods.Where(x => predicate(x as T)).Cast<T>().ToArray();
                case nameof(SellHistory): return Context.SellHistory.Where(x => predicate(x as T)).Cast<T>().ToArray();
                case nameof(Status): return Context.Statuses.Where(x => predicate(x as T)).Cast<T>().ToArray();
                case nameof(Town): return Context.Towns.Where(x => predicate(x as T)).Cast<T>().ToArray();
                default: throw new ArgumentException($"Указанного типа ({typename}) не существует в базе данных!");
            }
        }
    }
}
