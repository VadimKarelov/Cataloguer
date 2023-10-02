using Cataloguer.Database.Models;
using Cataloguer.Database.Repositories.Context;
using Cataloguer.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Cataloguer.Database.Repositories
{
    public class DistributionRepository : IRepository<Distribution>
    {
        private readonly CataloguerApplicationContext _context;
        private readonly DbSet<Distribution> _distributions;

        public DistributionRepository()
        {
            _context = new CataloguerApplicationContext();
            _distributions = _context.Distributions;
        }

        /// <summary>
        /// BrochureCount считается автоматически
        /// </summary>
        public async Task AddAsync(Distribution entity)
        {
            if (entity.Brochure == null && entity.BrochureId == Guid.Empty)
            {
                using var brochureRepository = new BrochureRepository();
                var brochure = new Brochure();
                await brochureRepository.AddAsync(brochure);
                entity.Brochure = brochure;
            }

            // AgeGroup и Gender не нужно создавать

            if (entity.Town == null && entity.TownId == Guid.Empty)
            {
                using var townRepository = new TownRepository();
                var town = new Town();
                await townRepository.AddAsync(town);
                entity.Town = town;
            }

            entity.BrochureCount = _context.BrochurePositions
                .Where(x => x.BrochureId == entity.BrochureId)
                .Count();

            _distributions.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Distribution entity)
        {
            _distributions.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.SaveChanges();
        }

        public IQueryable<Distribution>? TryGetAll()
        {
            return _distributions.AsQueryable();
        }

        public async Task<Distribution?> TryGetAsync(Guid guid)
        {
            return await _distributions.FirstOrDefaultAsync(x => x.Id == guid);
        }

        /// <summary>
        /// BrochureCount считается автоматически
        /// </summary>
        public async Task UpdateAsync(Distribution entity)
        {
            if (entity.Brochure == null && entity.BrochureId == Guid.Empty)
            {
                using var brochureRepository = new BrochureRepository();
                var brochure = new Brochure();
                await brochureRepository.AddAsync(brochure);
                entity.Brochure = brochure;
            }

            // AgeGroup и Gender не нужно создавать

            if (entity.Town == null && entity.TownId == Guid.Empty)
            {
                using var townRepository = new TownRepository();
                var town = new Town();
                await townRepository.AddAsync(town);
                entity.Town = town;
            }

            entity.BrochureCount = _context.BrochurePositions
                .Where(x => x.BrochureId == entity.BrochureId)
                .Count();

            _distributions.Update(entity);
            await _context.SaveChangesAsync();
        }
    }
}
