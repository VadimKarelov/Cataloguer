using Cataloguer.Database.Base;

namespace Cataloguer.Database.Models.Interfaces
{
    public interface ICataloguerModel<T> where T : ICataloguerModel<T>
    {
        internal T? Get(CataloguerApplicationContext context, int id, bool includeFields = false);

        internal IEnumerable<T?>? GetAll(CataloguerApplicationContext context, bool includeFields = false);

        internal IEnumerable<T?>? GetAll(CataloguerApplicationContext context, Func<T, bool> predicate, bool includeFields = false);
    }
}
