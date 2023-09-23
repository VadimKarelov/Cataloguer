using Cataloguer.Database.Models;
using Cataloguer.Database.Repositories.Context;
using Cataloguer.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Cataloguer.Database.Repositories
{
    /// <summary>
    /// Не хороший репозиторий, в репозиторий для доступа к таблицам товаров
    /// </summary>
    public class GoodRepository : IRepository<Good>
    {
        private readonly DbSet<Good> _goods;

        public GoodRepository()
        {
            var context = new CataloguerApplicationContext();
            _goods = context.Goods;
        }

        public void Add(Good entity)
        {
            throw new NotImplementedException();
        }

        public void Delete(Good entity)
        {
            throw new NotImplementedException();
        }

        public Good Get(Guid guid)
        {
            throw new NotImplementedException();
        }

        public IQueryable<Good> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Update(Good entity)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {

        }
    }
}
