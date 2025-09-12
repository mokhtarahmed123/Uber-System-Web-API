namespace Uber.Uber.Application.Interfaces
{
    public interface ICacheService
    {
        T Get<T>(string key);
        void Set<T>(string key, T value, TimeSpan duration);
        void Remove(string key);

        // Async methods (مفيدة للـ Redis)
        Task<T?> GetAsync<T>(string key);
        Task SetAsync<T>(string key, T value, TimeSpan duration);
        Task RemoveAsync(string key);
    }
}
