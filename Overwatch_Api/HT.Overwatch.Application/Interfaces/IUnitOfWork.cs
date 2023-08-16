namespace HT.Overwatch.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<T> GetRepository<T>() where T : class;

        int SaveChanges();

        Task<int> SaveChangesAsync();

        bool SaveInTransaction();
    }
}
