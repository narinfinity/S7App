using System.Linq;

namespace S7Test.Core.Interface.Common
{
    public interface IDataContext
    {
        IQueryable<TEntity> GetSet<TEntity>() where TEntity : class;
        void Attach<TEntity>(TEntity entity) where TEntity : class;
        void Detach<TEntity>(TEntity entity) where TEntity : class;
        TEntity Create<TEntity>(TEntity entity = null) where TEntity : class;
        void Delete<TEntity>(TEntity entity) where TEntity : class;
        bool Save();
    }
}
