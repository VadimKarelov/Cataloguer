namespace Cataloguer.Database.Repositories.Interfaces
{
    public interface IRepository<T>
    {
        public T Get(Guid guid);
        public void Add(T entity);
        public void Update(T entity);
        public void Delete(T entity);
    }
}
