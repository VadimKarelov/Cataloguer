namespace Cataloguer.Database.Repositories.Interfaces
{
    public interface IRepository<T> : IDisposable
    {
        public void Add(T entity);
        public void Delete(T entity);
        public T Get(Guid guid);
        public IQueryable<T> GetAll();
        public void Update(T entity);
    }
}
