using S7Test.Core.Entity;

namespace S7Test.Core.Interface.Common
{
    public interface IRepository<TEntity, TKey> : 
        IReader<TEntity, TKey>, 
        IFilteredOrderedPagedReader<TEntity, TKey>
    where TEntity : class, IEntity<TKey>
    {
        TEntity Create(TEntity entity = null);
        void Update(TEntity entity);
        void Delete(TKey id);
        void Save();
    }
}
