﻿using Cataloguer.Database.Models;
using Cataloguer.Database.Repositories.Context;
using Cataloguer.Database.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Cataloguer.Database.Repositories
{
    public class BrochureRepository : IRepository<Brochure>
    {
        private readonly CataloguerApplicationContext _context;
        private readonly DbSet<Brochure> _brochures;

        public BrochureRepository()
        {
            _context = new CataloguerApplicationContext();
            _brochures = _context.Brochures;
        }

        /// <summary>
        /// Поле PositionCount будет автоматически пересчитано
        /// </summary>
        public async Task AddAsync(Brochure entity)
        {
            entity.PositionCount = _context.BrochurePositions.Count(x => x.BrochureId == entity.Id);

            _brochures.Add(entity);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Brochure entity)
        {
            _brochures.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<Brochure?> TryGetAsync(Guid guid)
        {
            return await _brochures.FirstOrDefaultAsync(x => x.Id == guid);
        }

        public IQueryable<Brochure>? TryGetAll()
        {
            return _brochures.AsQueryable();
        }

        /// <summary>
        /// Поле PositionCount будет автоматически пересчитано
        /// </summary>
        public async Task UpdateAsync(Brochure entity)
        {
            entity.PositionCount = _context.BrochurePositions.Count(x => x.BrochureId == entity.Id);

            _brochures.Update(entity);
            await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.SaveChanges();
        }
    }
}