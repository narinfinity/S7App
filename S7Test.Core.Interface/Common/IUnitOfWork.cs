using S7Test.Core.Entity;

namespace S7Test.Core.Interface.Common
{
    public interface IUnitOfWork
    {
        void Dispose();        
        bool Save();
        IRepository<TEntity, TKey> Repository<TEntity, TKey>() where TEntity : class, IEntity<TKey>;
    }
}
