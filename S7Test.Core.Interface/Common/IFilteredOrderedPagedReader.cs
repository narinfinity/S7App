using S7Test.Core.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace S7Test.Core.Interface.Common
{
    public interface IFilteredOrderedPagedReader<TEntity, TKey>
       where TEntity : class, IEntity<TKey>
    {
        IQueryable<TEntity> GetList(Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            List<Expression<Func<TEntity, object>>> includeProperties = null,
            int? page = null,
            int? pageSize = null, bool tracking = true);
    }
}
