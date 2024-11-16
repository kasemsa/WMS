using System.Linq.Expressions;

namespace WarehouseManagementSystem.Contract.BaseRepository
{
    public interface IAsyncRepository<T> where T : class
    {
        IQueryable<T> Where(Expression<Func<T, bool>> predicate);
        Task<T> GetByIdAsync(int? id);
        Task<T?> GetByIdAsync(int id);
        Task<T> AddAsync(T entity);
        Task<T> AddAsync(T entity, string token);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);

        Task<T> FindAsync(Expression<Func<T, bool>> criteria);

    }
}
