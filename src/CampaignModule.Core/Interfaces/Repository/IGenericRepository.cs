namespace CampaignModule.Core.Interfaces.Repository;

public interface IGenericRepository<T> where T : class
{
    Task<T?> GetByCodeAsync(string code);
    Task<IEnumerable<T>?> GetAllAsync();
    Task<int> AddAsync(T entity);
    Task<int> UpdateAsync(T entity);
    Task<int> DeleteAsync(string code);
}