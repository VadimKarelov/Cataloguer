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
            if (entity.Good == null && entity.GoodId == Guid.Empty)
            {
                using var goodRepository = new GoodRepository();
                var good = new Good();
                await goodRepository.AddAsync(good);
                entity.Good = good;
            }

            if (entity.Town == null && entity.TownId == Guid.Empty)
            {
                using var townRepository = new TownRepository();
                var town = new Town();
                await townRepository.AddAsync(town);
                entity.Town = town;
            }

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

        public async Task<SellHistory?> TryGetAsync(Guid guid)
        {
            return await _sells.FirstOrDefaultAsync(x => x.Id == guid);
        }

        public async Task UpdateAsync(SellHistory entity)
        {
            if (entity.Good == null && entity.GoodId == Guid.Empty)
            {
                using var goodRepository = new GoodRepository();
                var good = new Good();
                await goodRepository.AddAsync(good);
                entity.Good = good;
            }

            if (entity.Town == null && entity.TownId == Guid.Empty)
            {
                using var townRepository = new TownRepository();
                var town = new Town();
                await townRepository.AddAsync(town);
                entity.Town = town;
            }

            _sells.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
