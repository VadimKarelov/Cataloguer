using Cataloguer.Database.Models;
using Cataloguer.Database.Repositories.Context;
using Cataloguer.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Cataloguer.Database.Repositories
{
    public class GenderRepository : IRepository<Gender>
    {
        private readonly CataloguerApplicationContext _context;
        private readonly DbSet<Gender> _genders;

        public GenderRepository()
        {
            _context = new CataloguerApplicationContext();
            _genders = _context.Genders;
        }

        public async Task AddAsync(Gender entity)
        {
            _genders.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Gender entity)
        {
            _genders.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.SaveChanges();
        }

        public IQueryable<Gender>? TryGetAll()
        {
            return _genders.AsQueryable();
        }

        public async Task<Gender?> TryGetAsync(Guid guid)
        {
            return await _genders.FirstOrDefaultAsync(x => x.Id == guid);
        }

        public async Task UpdateAsync(Gender entity)
        {
            _genders.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
