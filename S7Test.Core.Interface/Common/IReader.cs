using S7Test.Core.Entity;
using System.Linq;

namespace S7Test.Core.Interface.Common
{
    public interface IReader<out TEntity, in TKey>
       where TEntity : class, IEntity<TKey>
    {
        TEntity Get(TKey id, bool tracking = true);
        IQueryable<TEntity> GetList(bool tracking = true);
    }
}
