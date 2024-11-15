using System.Linq.Expressions;
using WarehouseManagementSystem.Models;
using WarehouseManagementSystem.Models.Common;

namespace WarehouseManagementSystem.Contract.BaseRepository
{
    public interface IAsyncRepository<T> where T : class
    {
        IQueryable<T> Where(Expression<Func<T, bool>> predicate);
        IQueryable<T> WhereThenFilter(Expression<Func<T, bool>> predicate, FilterObject filterObject);
        Task<IReadOnlyList<T>> GetWhereThenPagedReponseAsync(Expression<Func<T, bool>> predicate, int page, int size);
        Task<IReadOnlyList<T>> GetWhereThenPagedReponseAsync(Expression<Func<T, bool>> predicate, FilterObject filterObject, int page, int size);
        IQueryable<T> Filtration(FilterObject filterObject);
        Task<IReadOnlyList<T>> GetFilterThenPagedReponseAsync(FilterObject filterObject, int page, int size);
        Task<List<string>> GetPropertyNames();
        Task<IReadOnlyList<T>> ListAllAsync();
        IQueryable<T> ListAllAsync(FilterObject filterObject);
        Task<T> GetByIdAsync(int? id);
        Task<T?> GetByIdAsync(int id);
        Task<T> AddAsync(T entity);
        Task<T> AddAsync(T entity, string token);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<int> GetCountAsync(Expression<Func<T, bool>>? predicate);
        Task<IReadOnlyList<T>> GetPagedReponseAsync(int page, int size);
        Task<IReadOnlyList<T>> GetPagedReponseWithPredicateAsync(Expression<Func<T, bool>>? predicate, int page, int size);

    }
}
