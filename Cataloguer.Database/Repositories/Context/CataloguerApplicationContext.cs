using Cataloguer.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Cataloguer.Database.Repositories.Context
{
    /// <summary>
    /// Можно использовать только внутри проекта Database
    /// </summary>
    internal class CataloguerApplicationContext : DbContext
    {
        public DbSet<AgeGroup> AgeGroups { get; set; }
        public DbSet<Brochure> Brochures { get; set; }
        public DbSet<BrochurePosition> BrochurePositions { get; set; }
        public DbSet<Distribution> Distributions { get; set; }
        public DbSet<Gender> Genders { get; set; }
        public DbSet<Good> Goods { get; set; }
        public DbSet<SellHistory> SellHistory { get; set; }
        public DbSet<Status> Statuses { get; set; }
        public DbSet<Town> Towns { get; set; }   

        public CataloguerApplicationContext()
        {
            Database.EnsureCreated();
        }
    }
}
