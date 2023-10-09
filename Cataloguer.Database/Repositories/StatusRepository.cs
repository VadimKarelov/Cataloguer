using Cataloguer.Database.Models;
using Cataloguer.Database.Repositories.Context;
using Cataloguer.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Cataloguer.Database.Repositories
{
    public class StatusRepository : IRepository<Status>
    {
        private readonly CataloguerApplicationContext _context;
        private readonly DbSet<Status> _statuses;

        public StatusRepository()
        {
            _context = new CataloguerApplicationContext();
            _statuses = _context.Statuses;
        }

        public async Task AddAsync(Status entity)
        {
            _statuses.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Status entity)
        {
            _statuses.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.SaveChanges();
        }

        public IQueryable<Status>? TryGetAll()
        {
            return _statuses.AsQueryable();
        }

        public async Task<Status?> TryGetAsync(int id)
        {
            return await _statuses.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task UpdateAsync(Status entity)
        {
            _statuses.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
