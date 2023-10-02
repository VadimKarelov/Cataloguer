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

        public async Task AddAsync(Good entity)
        {
            _goods.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Good entity)
        {
            _goods.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<Good?> TryGetAsync(Guid guid)
        {
            return await _goods.FirstOrDefaultAsync(x => x.Id == guid);
        }

        public IQueryable<Good>? TryGetAll()
        {
            return _goods.AsQueryable();
        }

        public async Task UpdateAsync(Good entity)
        {
            _goods.Update(entity);
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.SaveChanges();
        }
    }
}
