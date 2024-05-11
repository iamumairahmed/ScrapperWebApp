using ScrapperWebApp.Repository;

namespace ScrapperWebApp.UnitOfWork
{
    public interface IUnitOfWork
    {
        IRepository<T> Repository<T>() where T : class;
        Task<int> SaveAsync();
        int Save();
        void BeginTransaction();
        void CommitTransaction();
        void RollbackTransaction();
    }
}
