using Cataloguer.Database.Models;
using Cataloguer.Database.Repositories.Context;
using Cataloguer.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Cataloguer.Database.Repositories
{
    public class AgeGroupRepository : IRepository<AgeGroup>
    {
        private readonly CataloguerApplicationContext _context;
        private readonly DbSet<AgeGroup> _groups;

        public AgeGroupRepository()
        {
            _context = new CataloguerApplicationContext();
            _groups = _context.AgeGroups;
        }

        public async Task AddAsync(AgeGroup entity)
        {
            _groups.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(AgeGroup entity)
        {
            _groups.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.SaveChanges();
        }

        public IQueryable<AgeGroup>? TryGetAll()
        {
            return _groups.AsQueryable();
        }

        public async Task<AgeGroup?> TryGetAsync(Guid guid)
        {
            return await _groups.FirstOrDefaultAsync(x => x.Id == guid);
        }

        public async Task UpdateAsync(AgeGroup entity)
        {
            _groups.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
