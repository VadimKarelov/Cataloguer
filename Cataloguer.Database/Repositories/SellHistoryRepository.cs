using Cataloguer.Database.Models;
using Cataloguer.Database.Repositories.Context;
using Cataloguer.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Cataloguer.Database.Repositories
{
    public class SellHistoryRepository : IRepository<SellHistory>
    {
        private readonly CataloguerApplicationContext _context;
        private readonly DbSet<SellHistory> _sells;

        public SellHistoryRepository()
        {
            _context = new CataloguerApplicationContext();
            _sells = _context.SellHistory;
        }

        public async Task AddAsync(SellHistory entity)
        {
            await CreateIfForeignEntetiesDoNotExistAsync(entity);

            _sells.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(SellHistory entity)
        {
            _sells.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.SaveChanges();
        }

        public IQueryable<SellHistory>? TryGetAll()
        {
            return _sells.AsQueryable();
        }

        public async Task<SellHistory?> TryGetAsync(int id)
        {
            return await _sells.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task UpdateAsync(SellHistory entity)
        {
            await CreateIfForeignEntetiesDoNotExistAsync(entity);

            _sells.Update(entity);
            await _context.SaveChangesAsync();
        }

        private async Task CreateIfForeignEntetiesDoNotExistAsync(SellHistory entity)
        {
            if (entity.Good == null && !_context.Goods.Any(x => x.Id == entity.GoodId))
            {
                using var goodRepository = new GoodRepository();
                var good = new Good();
                await goodRepository.AddAsync(good);
                entity.Good = good;
            }

            if (entity.Town == null && !_context.Towns.Any(x => x.Id == entity.TownId))
            {
                using var townRepository = new TownRepository();
                var town = new Town();
                await townRepository.AddAsync(town);
                entity.Town = town;
            }
        }
    }
}
