namespace Uber.Uber
{
    public interface CommonWithDatabase<T>
    {
        Task Create(T entity);
        Task Delete(int id);
        Task<T> Update(int ID, T entity);
        Task<T> GetByID(int ID);
        Task<List<T>> FindAll(int page = 1, int pageSize = 20);
        Task SaveChange();
    }
}
