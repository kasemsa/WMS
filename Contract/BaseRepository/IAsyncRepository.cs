using System.Linq.Expressions;
using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.Models.Common;

namespace WarehouseManagementSystem.Contract.BaseRepository
{
    public interface IAsyncRepository<T> where T : class
    {
        // Filtering and Querying
        IQueryable<T> Where(Expression<Func<T, bool>> predicate);
        IQueryable<T> WhereThenFilter(Expression<Func<T, bool>> predicate, FilterObject filterObject);
        IQueryable<T> Filtration(FilterObject filterObject);
        Task<IReadOnlyList<T>> GetFilterThenPagedReponseAsync(FilterObject filterObject, int page, int size);

        // Paging
        Task<IReadOnlyList<T>> GetWhereThenPagedReponseAsync(Expression<Func<T, bool>> predicate, int page, int size);
        Task<IReadOnlyList<T>> GetWhereThenPagedReponseAsync(Expression<Func<T, bool>> predicate, FilterObject filterObject, int page, int size);
        Task<IReadOnlyList<T>> GetPagedReponseAsync(int page, int size);
        Task<IReadOnlyList<T>> GetPagedReponseWithPredicateAsync(Expression<Func<T, bool>>? predicate, int page, int size);

        // Count
        Task<int> GetCountAsync(Expression<Func<T, bool>>? predicate);

        // CRUD Operations
        Task<IReadOnlyList<T>> ListAllAsync();
        IQueryable<T> ListAllAsync(FilterObject filterObject);
        Task<T> GetByIdAsync(int? id);
        Task<T?> GetByIdAsync(int id);
        Task<T> AddAsync(T entity);
        Task<T> AddAsync(T entity, string token);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);

        // Additional Utilities
        Task<List<string>> GetPropertyNames();
        Task<T> FindAsync(Expression<Func<T, bool>> criteria); // Ensure this is included
    }
}
