using Cataloguer.Database.Models;
using Microsoft.EntityFrameworkCore;

namespace Cataloguer.Database.Repositories.Context
{
    internal class CataloguerApplicationContext : DbContext
    {
        public DbSet<Good> Goods { get; set; }

        public CataloguerApplicationContext()
        {
            Database.EnsureCreated();

            Goods = Set<Good>();
        }
    }
}
