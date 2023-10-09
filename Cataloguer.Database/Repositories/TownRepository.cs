using Cataloguer.Database.Models;
using Cataloguer.Database.Repositories.Context;
using Cataloguer.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Cataloguer.Database.Repositories
{
    public class TownRepository : IRepository<Town>
    {
        private readonly CataloguerApplicationContext _context;
        private readonly DbSet<Town> _towns;

        public TownRepository()
        {
            _context = new CataloguerApplicationContext();
            _towns = _context.Towns;
        }

        public async Task AddAsync(Town entity)
        {
            _towns.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Town entity)
        {
            _towns.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public IQueryable<Town>? TryGetAll()
        {
            return _towns.AsQueryable();
        }

        public async Task<Town?> TryGetAsync(int id)
        {
            return await _towns.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task UpdateAsync(Town entity)
        {
            _towns.Update(entity);
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.SaveChanges();
        }
    }
}
