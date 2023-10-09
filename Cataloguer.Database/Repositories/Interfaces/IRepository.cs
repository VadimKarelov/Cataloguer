namespace Cataloguer.Database.Repositories.Interfaces
{
    public interface IRepository<T> : IDisposable
    {
        public Task AddAsync(T entity);
        public Task DeleteAsync(T entity);
        public Task<T?> TryGetAsync(int id);
        public IQueryable<T>? TryGetAll();
        public Task UpdateAsync(T entity);
    }
}
