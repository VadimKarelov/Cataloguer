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
        private readonly CataloguerApplicationContext _context;
        private readonly DbSet<Good> _goods;

        public GoodRepository()
        {
            _context = new CataloguerApplicationContext();
            _goods = _context.Goods;
        }

        public void Add(Good entity)
        {
            _goods.Add(entity);
            _context.SaveChanges();
        }

        public void Delete(Good entity)
        {
            _goods.Remove(entity);
            _context.SaveChanges();
        }

        public async Task<Good?> TryGetAsync(Guid guid)
        {
            return await _goods.FirstOrDefaultAsync();
        }

        public IQueryable<Good>? TryGetAll()
        {
            return _goods;
        }

        public void Update(Good entity)
        {
            _goods.Update(entity);
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.SaveChanges();
        }
    }
}
