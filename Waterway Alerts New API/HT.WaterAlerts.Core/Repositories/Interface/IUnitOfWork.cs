namespace HT.WaterAlerts.Core
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<T> GetRepository<T>() where T : class;

        int SaveChanges();

        Task<int> SaveChangesAsync();

        bool SaveInTransaction();
    }
}