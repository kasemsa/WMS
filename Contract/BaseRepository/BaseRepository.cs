using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Transactions;
using WarehouseManagementSystem.DataBase;

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
                    if (typeof(T).GetProperty("isDeleted") != null)
                    {
                        typeof(T).GetProperty("isDeleted")!.SetValue(entity, true);
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
    }
}
