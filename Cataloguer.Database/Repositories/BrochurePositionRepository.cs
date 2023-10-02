using Cataloguer.Database.Models;
using Cataloguer.Database.Repositories.Context;
using Cataloguer.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace Cataloguer.Database.Repositories
{
    public class BrochurePositionRepository : IRepository<BrochurePosition>
    {
        private readonly CataloguerApplicationContext _context;
        private readonly DbSet<BrochurePosition> _positions;

        public BrochurePositionRepository()
        {
            _context = new CataloguerApplicationContext();
            _positions = _context.BrochurePositions;
        }

        public async Task AddAsync(BrochurePosition entity)
        {
            if (entity.Brochure == null && entity.BrochureId == Guid.Empty)
            {
                using var brochureRepository = new BrochureRepository();
                var brochure = new Brochure();
                await brochureRepository.AddAsync(brochure);
                entity.Brochure = brochure;
            }

            if (entity.Good == null && entity.GoodId == Guid.Empty)
            {
                using var goodRepository = new GoodRepository();
                var good = new Good();
                await goodRepository.AddAsync(good);
                entity.Good = good;
            }

            _positions.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(BrochurePosition entity)
        {
            _positions.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.SaveChanges();
        }

        public IQueryable<BrochurePosition>? TryGetAll()
        {
            return _positions.AsQueryable();
        }

        public async Task<BrochurePosition?> TryGetAsync(Guid guid)
        {
            return await _positions.FirstOrDefaultAsync(x => x.Id == guid);
        }

        public async Task UpdateAsync(BrochurePosition entity)
        {
            if (entity.Brochure == null && entity.BrochureId == Guid.Empty)
            {
                using var brochureRepository = new BrochureRepository();
                var brochure = new Brochure();
                await brochureRepository.AddAsync(brochure);
                entity.Brochure = brochure;
            }

            if (entity.Good == null && entity.GoodId == Guid.Empty)
            {
                using var goodRepository = new GoodRepository();
                var good = new Good();
                await goodRepository.AddAsync(good);
                entity.Good = good;
            }

            _positions.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
