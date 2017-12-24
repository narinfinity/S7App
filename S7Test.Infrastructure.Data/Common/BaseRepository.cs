using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using S7Test.Core.Entity;
using S7Test.Core.Interface.Common;

namespace S7Test.Infrastructure.Data.Common
{
    public abstract class BaseRepository<TEntity, TKey, TContext> : 
        IRepository<TEntity, TKey>,
        IDisposable
     where TEntity : class, IEntity<TKey>
     where TContext : DbContext, IDataContext

    {
        protected TContext Context;

        protected BaseRepository(TContext context) { Context = context; }

        //Method implementations
        public IQueryable<TEntity> GetList(bool tracking = true)
        {
            return tracking
                ? Context.GetSet<TEntity>()
                : Context.GetSet<TEntity>().AsNoTracking();
        }
        public IQueryable<TEntity> GetList(Expression<Func<TEntity, bool>> filter,
                    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
                    List<Expression<Func<TEntity, object>>> includeProperties = null, 
                    int? page = null, 
                    int? pageSize = null, 
                    bool tracking = true)
        {
            var query = Context.GetSet<TEntity>();
           
            includeProperties?.ForEach(i => { query = query.Include(i); });

            if (filter != null)
                query = query.Where(filter);

            if (orderBy != null)
                query = orderBy(query);

            if (page != null && pageSize != null)
                query = query
                    .Skip((page.Value - 1) * pageSize.Value)
                    .Take(pageSize.Value);

            return tracking
                ? query
                : query.AsNoTracking();
        }

        public TEntity Get(TKey id, bool tracking = true)
        {
            var entity = Context.Set<TEntity>().Find(id);
            if (entity != null && !tracking) Context.Detach(entity);
            return entity;
        }

        public void Update(TEntity entity)
        {
            Context.Attach<TEntity>(entity);
        }

        public virtual TEntity Create(TEntity entity = null)
        {
            return Context.Create(entity);
        }

        public void Save()
        {
            Context.Save();
        }

        public void Delete(TKey id)
        {
            var entity = Get(id, true);
            Context.Delete(entity);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        protected virtual void Dispose(bool disposing)
        {
            //Clean up managable resources
            if (disposing)
            {
                if (Context != null)
                {
                    Context.Dispose();
                    Context = null;
                }
            }
            //Clean up unmanagable resources

        }

    }


}
