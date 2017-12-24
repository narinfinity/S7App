using System.Linq;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using S7Test.Core.Entity;
using S7Test.Core.Entity.App;
using S7Test.Core.Interface.Common;
using S7Test.Infrastructure.Data.EntityConfiguration;
using Microsoft.AspNetCore.Identity;
using S7Test.Core.Entity.Domain;

namespace S7Test.Infrastructure.Data
{
    public class AppDbContext : IdentityDbContext<User, IdentityRole<string>, string,
        IdentityUserClaim<string>,
        IdentityUserRole<string>,
        IdentityUserLogin<string>,
        IdentityRoleClaim<string>,
        IdentityUserToken<string>>, IDataContext
    {
        static AppDbContext() { }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // Add your customizations after calling base.OnModelCreating(builder);

            //builder.UseOpenIddict<string>();
            builder.AddEntityConfigurations();
            builder.AddOpenIddictEntityConfigurations();
        }
        //DbSets
        public DbSet<Player> Players { get; set; }
        public DbSet<Team> Teams { get; set; }

        public IQueryable<TEntity> GetSet<TEntity>() where TEntity : class
        {
            return Set<TEntity>();
        }

        public new void Attach<TEntity>(TEntity entity) where TEntity : class
        {
            Set<TEntity>().Attach(entity);
            Entry<TEntity>(entity).State = EntityState.Modified;
        }

        public void Detach<TEntity>(TEntity entity) where TEntity : class
        {
            if (Set<TEntity>().Local.Contains(entity))
            {
                Entry<TEntity>(entity).State = EntityState.Detached;
            }
        }

        public TEntity Create<TEntity>(TEntity entity = null) where TEntity : class
        {
            if (entity != null)
            {
                Set<TEntity>().Attach(entity);
            }
            else
            {
                entity = Set<TEntity>().Add(entity).Entity;
            }
            Entry<TEntity>(entity).State = EntityState.Added;
            return entity;
        }

        public void Delete<TEntity>(TEntity entity) where TEntity : class
        {
            Set<TEntity>().Remove(entity);
        }

        public bool Save()
        {
            var entries = ChangeTracker.Entries().Where(e => e.State == EntityState.Modified || e.State == EntityState.Added || e.State == EntityState.Unchanged);

            foreach (var entry in entries)
            {
                if (entry.Entity is IEntity<int> entityInt)
                {
                    entry.State = entityInt.Id > 0 ? EntityState.Modified : EntityState.Added;
                }
                
                else if (entry.Entity is IdentityUser<string> entity)
                {
                    entry.State = entity.Id != null ? EntityState.Modified : EntityState.Added;
                }

            }
            return SaveChanges(acceptAllChangesOnSuccess: true) > 0;
        }


        protected virtual void Dispose(bool disposing)
        {
            //Clean up managable resources
            if (disposing)
            {

            }
            //Clean up unmanagable resources

            base.Dispose();
        }
        ~AppDbContext()
        {
            Dispose(false);
        }
    }
}
