using HT.Overwatch.Application.Interfaces;

namespace HT.Overwatch.Infrastructure.Persistence
{
    public class UnitOfWork : IUnitOfWork
    {
        private OverwatchDbContext _dbContext;

        private Dictionary<Type, object> _repos = default!;

        private bool _disposed = false;

        public UnitOfWork(OverwatchDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException("Context was not supplied");
        }

        public IRepository<T> GetRepository<T>() where T : class
        {
            if (_repos == null)
            {
                _repos = new Dictionary<Type, object>();
            }

            var type = typeof(T);
            if (!_repos.ContainsKey(type))
            {
                _repos[type] = new Repository<T>(_dbContext);
            }

            return (IRepository<T>)_repos[type];
        }

        public int SaveChanges() => _dbContext.SaveChanges();

        public Task<int> SaveChangesAsync() => _dbContext.SaveChangesAsync();

        public bool SaveInTransaction()
        {
            using var transaction = _dbContext.Database.BeginTransaction();
            try
            {
                SaveChanges();
                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                return false;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    _dbContext.Dispose();
                    _dbContext = null;
                    this._disposed = true;
                }
            }
        }
    }
}
