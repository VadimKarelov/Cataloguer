namespace Cataloguer.Database.Repositories.Interfaces
{
    public interface IRepository<T> : IDisposable
    {
        public void Add(T entity);
        public void Delete(T entity);
        public Task<T?> TryGetAsync(Guid guid);
        public IQueryable<T>? TryGetAll();
        public void Update(T entity);
    }
}
