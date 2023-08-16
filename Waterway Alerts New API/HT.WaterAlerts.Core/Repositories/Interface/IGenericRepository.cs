using Microsoft.Data.SqlClient;
using System.Linq.Expressions;

namespace HT.WaterAlerts.Core
{
    public interface IGenericRepository<T> where T : class
    {
        IQueryable<T> GetQuerable();

        T GetById(object id);

        T GetSingle(Expression<Func<T, bool>> criteria);

        T GetFirst(Expression<Func<T, bool>> criteria);

        IQueryable<T> GetAll();

        IQueryable<T> Get(Expression<Func<T, bool>> criteria);

        IQueryable<T> Get(Expression<Func<T, object>> orderBy, SortOrder sortOrder);

        IQueryable<T> Get(Expression<Func<T, bool>> criteria, Expression<Func<T, object>> orderBy, SortOrder sortOrder);

        IQueryable<T> Get(Expression<Func<T, object>> orderBy, SortOrder sortOrder, int pageIndex, int pageSize);

        IQueryable<T> Get(Expression<Func<T, bool>> criteria, Expression<Func<T, object>> orderBy, SortOrder sortOrder, int pageIndex, int pageSize);

        IEnumerable<T> GetWithRawSql(string query, params object[] parameters);

        int Count();

        int Count(Expression<Func<T, bool>> criteria);

        void Add(T entity);

        void Add(IEnumerable<T> entities);

        void Update(T entity);

        void Delete(T entity);

        void Delete(IEnumerable<T> entities);

        /*IQueryable<T> GetAll(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            List<Expression<Func<T, object>>> includes = null,
            int? page = null,
            int? pageSize = null);

        abstract IQueryable<T> GetAll();

        IQueryable<T> GetAllA();

        Task<IEnumerable<T>> GetAllAsync(
          Expression<Func<T, bool>> filter = null,
          Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
          List<Expression<Func<T, object>>> includes = null,
          int? page = null,
          int? pageSize = null);

        IQueryable<T> Find(Expression<Func<T, bool>> predicate);

        void Insert(T entity);

        void InsertRange(IEnumerable<T> entities);

        void Update(T entity);

        void Delete(Guid userId, List<Guid> roles);

        void Delete(object id);

        void Delete(T entity);

        void DeleteAsync(object id);

        void DeleteRange(IEnumerable<T> entities);

        IQueryable<T> RawSql(string query, params object[] parameters);*/
    }
}