using AutoMapper.Internal;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;
using System.Transactions;
using WarehouseManagementSystem.DataBase;
using WarehouseManagementSystem.Models.Common;

namespace WarehouseManagementSystem.Contract.BaseRepository
{
    public class BaseRepository<T> : IAsyncRepository<T> where T : class
    {
        protected WarehouseDbContext _dbContext { get; set; }
        protected readonly DbSet<T> _DbSet;

        public BaseRepository(WarehouseDbContext dbContext)
        {
            _dbContext = dbContext;
            _DbSet = _dbContext.Set<T>();
        }

        public virtual async Task<T?> GetByIdAsync(int? id)
        {
            T? t = await _DbSet.FindAsync(id);
            return t;
        }
        public virtual async Task<T?> GetByIdAsync(int id)
        {
            T? t = await _DbSet.FindAsync(id);
            return t;
        }

        public async Task<T> FindAsync(Expression<Func<T, bool>> criteria)
        {
            return await _DbSet.SingleOrDefaultAsync(criteria);
        }


        public async Task<T> AddAsync(T entity)
        {
            await _DbSet.AddAsync(entity);

            if (typeof(T).GetProperty("CreatedAt") != null)
            {
                typeof(T).GetProperty("CreatedAt")!.SetValue(entity, DateTime.UtcNow);
            }

            await _dbContext.SaveChangesAsync();

            return entity;
        }
        public async Task<T> AddAsync(T entity, string UserName)
        {
            await _DbSet.AddAsync(entity);

            if (typeof(T).GetProperty("CreatedAt") != null)
            {
                typeof(T).GetProperty("CreatedAt")!.SetValue(entity, DateTime.UtcNow);
                typeof(T).GetProperty("CreatedBy")!.SetValue(entity, UserName);
            }

            await _dbContext.SaveChangesAsync();

            return entity;
        }

        public async Task UpdateAsync(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteRange(Expression<Func<T, bool>>? predicate)
        {
            _DbSet.RemoveRange(_DbSet.Where(predicate!));
        }

        public async Task DeleteAsync(T entity)
        {
            TransactionOptions TransactionOptions = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted,
                Timeout = TimeSpan.FromMinutes(5)
            };

            using (TransactionScope Transaction2 = new TransactionScope(TransactionScopeOption.Required,
                TransactionOptions, TransactionScopeAsyncFlowOption.Enabled))
            {
                try
                {
                    if (typeof(T).GetProperty("IsDeleted") != null)
                    {
                        typeof(T).GetProperty("IsDeleted")!.SetValue(entity, true);
                        typeof(T).GetProperty("DeletedAt")!.SetValue(entity, DateTime.UtcNow);
                    }

                    _dbContext.Entry(entity).State = EntityState.Modified;

                    await _dbContext.SaveChangesAsync();

                    Transaction2.Complete();
                }
                catch (Exception)
                {
                    Transaction2.Dispose();
                    throw;
                }
            }
        }

        //public async Task<User> GetUserByUserName(string userName)
        //{
        //    var User = await _dbContext.Users.Where(u => u.UserName == userName).FirstOrDefaultAsync();
        //    return User;
        //}

        public IQueryable<T> Where(Expression<Func<T, bool>> predicate)
        {
            return _DbSet.AsNoTracking().Where(predicate);
        }

        public IQueryable<T> WhereThenFilter(Expression<Func<T, bool>> predicate, FilterObject filterObject)
        {
            IQueryable<T> query = _DbSet.AsNoTracking().Where(predicate);

            if (filterObject != null && filterObject.Filters != null)
            {
                query = Filtration(filterObject);
            }

            return query.Where(predicate);
        }

        public async Task<IReadOnlyList<T>> GetWhereThenPagedReponseAsync(Expression<Func<T, bool>> predicate, int page, int size)
        {
            if (size == -1 || page == 0)
                return await _DbSet.AsNoTracking().Where(predicate).ToListAsync();
            if (size == 0)
                size = 10;
            return await _DbSet.AsNoTracking()
                .Where(predicate).Skip((page - 1) * size).Take(size).ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetWhereThenPagedReponseAsync(Expression<Func<T, bool>> predicate, FilterObject filterObject, int page, int size)
        {
            var query = await _DbSet.AsNoTracking().Where(predicate).ToListAsync();

            if (size == -1 || page == 0)
            {
                query = await _DbSet.AsNoTracking().Where(predicate).ToListAsync();
            }
            else
            {
                if (size == 0)
                    size = 10;
                query = query.Skip((page - 1) * size).Take(size).ToList();
            }

            if (filterObject != null && filterObject.Filters != null)
            {
                query = Filtration(filterObject).ToList();
            }



            return query;
        }

        public async Task<IReadOnlyList<T>> GetFilterThenPagedReponseAsync(FilterObject filterObject, int page, int size)
        {
            var query = await _DbSet.AsNoTracking().ToListAsync();

            if (size == -1 || page == 0)
            {
                query = await _DbSet.AsNoTracking().ToListAsync();
            }

            if (filterObject != null && filterObject.Filters != null)
            {
                query = Filtration(filterObject).ToList();
            }
            if (size != -1)
            {
                if (size == 0)
                    size = 10;
                query = query
                    .Skip((page - 1) * size).Take(size).ToList();
            }

            return query;
        }

        public async Task<IReadOnlyList<T>> ListAllAsync()
        {
            return await _DbSet.AsNoTracking().ToListAsync();
        }

        public IQueryable<T> ListAllAsync(FilterObject filterObject)
        {
            IQueryable<T> query = _DbSet.AsNoTracking();

            if (filterObject != null && filterObject.Filters != null)
            {
                query = Filtration(filterObject);
            }

            return query;
        }

        public async Task<int> GetCountAsync(Expression<Func<T, bool>>? predicate)
        {
            if (predicate == null)
                return await _DbSet.CountAsync();

            return await _DbSet.AsNoTracking().Where(predicate).CountAsync();
        }

        public async Task<IReadOnlyList<T>> GetPagedReponseAsync(int page, int size)
        {
            if (size == -1 || page == 0)
                return await _DbSet.AsNoTracking().ToListAsync();
            if (size == 0)
                size = 10;
            return await _DbSet.AsNoTracking().Skip((page - 1) * size).Take(size).ToListAsync();
        }

        public async Task<IReadOnlyList<T>> GetPagedReponseWithPredicateAsync(Expression<Func<T, bool>>? predicate, int page, int size)
        {
            if (size == -1 || page == 0)
                return await _DbSet.AsNoTracking().Where(predicate!).ToListAsync();
            if (size == 0)
                size = 10;
            return await _DbSet.AsNoTracking().Where(predicate!).Skip((page - 1) * size).Take(size).ToListAsync();
        }
        public IQueryable<T> Filtration(FilterObject filterObject)
        {
            IQueryable<T> query = _DbSet.AsNoTracking();

            foreach (var filter in filterObject.Filters!)
            {
                var propertyType = typeof(T).GetProperty(filter.Key!)?.PropertyType;

                if (propertyType != null)
                {
                    if (filter.Value == null && filter.DateRange == null)
                    {
                        // Process null value
                        query = query.Where(entity => EF.Property<object>(entity, filter.Key!) == null);
                    }
                    else if (propertyType == typeof(string) && filter.Key == "Time" && filter.Value is string TimeValue)
                    {
                        // Process string value
                        if (!string.IsNullOrEmpty(TimeValue))
                        {

                            var query1 = query.Where(entity => EF.Property<string>(entity, filter.Key!).StartsWith(TimeValue));
                            var query2 = query.Where(entity => EF.Property<string>(entity, filter.Key!).EndsWith(TimeValue));
                            query = query1.Union(query2);
                        }
                    }

                    else if (propertyType == typeof(string) && filter.Value is string stringValue)
                    {
                        // Process string value
                        if (!string.IsNullOrEmpty(stringValue))
                        {
                            query = query.Where(entity => EF.Property<string>(entity, filter.Key!).Contains(stringValue));
                        }
                    }
                    else if (propertyType == typeof(int) && filter.Value is string IntValue)
                    {
                        int Value = int.Parse(IntValue);
                        // Process string value
                        if (!string.IsNullOrEmpty(IntValue))
                        {
                            query = query.Where(entity => EF.Property<int>(entity, filter.Key!) == Value);
                        }
                    }
                    else if (propertyType == typeof(bool) && filter.Value is string boolValue)
                    {
                        bool Value = bool.Parse(boolValue);

                        query = query.Where(entity => EF.Property<bool>(entity, filter.Key!) == Value);
                    }
                    else if (propertyType == typeof(Nullable<bool>) && filter.Value is null)
                    {
                        query = query.Where(entity => EF.Property<bool>(entity, filter.Key!) == null);
                    }
                    else if (propertyType == typeof(Nullable<bool>) && filter.Value is string NullableValue)
                    {
                        bool Value = bool.Parse(NullableValue);
                        query = query.Where(entity => EF.Property<bool>(entity, filter.Key!) == Value);
                    }
                    else if (propertyType == typeof(DateTime) && filter.Key == "CreatedAt" && filter.DateRange is DateTimeRange CreatedDate)
                    {
                        query = query.Where(entity =>
                               EF.Property<DateTime>(entity, filter.Key) == CreatedDate.StartDate);
                    }
                    else if (propertyType == typeof(DateTime) && filter.DateRange is DateTimeRange dateRange)
                    {
                        // Process date range
                        if (dateRange.StartDate != null && dateRange.EndDate != null)
                        {
                            query = query.Where(entity =>
                                EF.Property<DateTime>(entity, filter.Key) >= dateRange.StartDate &&
                                EF.Property<DateTime>(entity, filter.Key) <= dateRange.EndDate);
                        }
                        else if (dateRange.StartDate != null)
                        {
                            query = query.Where(entity =>
                                EF.Property<DateTime>(entity, filter.Key) >= dateRange.StartDate);
                        }
                        else if (dateRange.EndDate != null)
                        {
                            query = query.Where(entity =>
                                EF.Property<DateTime>(entity, filter.Key) <= dateRange.EndDate);
                        }
                    }
                    else
                    {
                        // Process other value types
                        query = query.Where(entity => EF.Property<object>(entity, filter.Key!).Equals(filter.Value));
                    }
                }
            }
            return query;
        }
        public async Task<List<string>> GetPropertyNames()
        {
            List<string> PropertyNames = new List<string>();
            foreach (PropertyInfo propertyInfo in typeof(T).GetProperties())
            {
                if (!propertyInfo.GetModifiedPropertyType().IsListType())
                    PropertyNames.Add(propertyInfo.Name);
            }
            return PropertyNames;
        }

        public IQueryable<T> AsQueryable()
        {
            return _DbSet.AsNoTracking();
        }


    }
}
