using System;
using System.Collections.Generic;
using S7Test.Core.Entity;
using S7Test.Core.Interface.Common;

namespace S7Test.Infrastructure.Data.Common
{
    public class UnitOfWork : IUnitOfWork, IDisposable
    {
        private IDataContext _context;
        private IDictionary<string, object> _baseRepositories;
        private IDictionary<string, object> _repositories;

        public UnitOfWork(IDataContext context)
        {
            _context = context;
            _baseRepositories = new Dictionary<string, object>();
            _repositories = new Dictionary<string, object>();
        }

        public bool Save()
        {
            return _context.Save();
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
                if (_baseRepositories != null)
                {
                    foreach (var key in _baseRepositories.Keys)
                        if (_baseRepositories[key] != null) ((IDisposable)_baseRepositories[key]).Dispose();
                    _baseRepositories = null;
                }

                if (_repositories != null)
                {
                    foreach (var key in _repositories.Keys)
                        if (_repositories[key] != null) ((IDisposable)_repositories[key]).Dispose();
                    _repositories = null;
                }
                _context = null;
            }
            //Clean up unmanagable resources

        }

        public IRepository<TEntity, TKey> Repository<TEntity, TKey>() where TEntity : class, IEntity<TKey>
        {
            var key = typeof(TEntity).Name;
            if (!_baseRepositories.ContainsKey(key)) _baseRepositories.Add(key, new Repository<TEntity, TKey>(_context));
            return (IRepository<TEntity, TKey>)_baseRepositories[key];
        }






    }
}
