using S7Test.Core.Entity;
using S7Test.Core.Interface.Common;

namespace S7Test.Infrastructure.Data.Common
{
    public class Repository<TEntity, TKey> : BaseRepository<TEntity, TKey, AppDbContext>
       where TEntity : class, IEntity<TKey>
    {
        public Repository(IDataContext context) : base((AppDbContext)context) {}
        
        protected override void Dispose(bool disposing)
        {
            
            //Clean up managable resources
            if (disposing)
            {
                
            }
            //Clean up unmanagable resources
            
            base.Dispose(disposing);
        }
        ~Repository()
        {
            Dispose(false);
        }
    }
}
