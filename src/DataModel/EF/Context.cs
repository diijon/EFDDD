using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;

namespace EFDDD.DataModel.EF
{
    public class Context : DbContext, IContext
    {
        public IDbSet<Home> Home { get; set; }
        public IDbSet<HomeOwner> HomeOwners { get; set; }
        
        public Context() { }

        public Context(string nameOrConnectionString)
            : base(nameOrConnectionString) { }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(Constants.DB_SCHEMA);
        }
    }

    public interface IContext
    {
        DbChangeTracker ChangeTracker { get; }
        DbContextConfiguration Configuration { get; }
        Database Database { get; }

        DbSet<TEntity> Set<TEntity>() where TEntity : class;
        DbSet Set(Type entityType);
        IEnumerable<DbEntityValidationResult> GetValidationErrors();
        DbEntityEntry Entry(object entity);
        DbEntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
        int SaveChanges();
        void Dispose();

        IDbSet<Home> Home { get; set; }
        IDbSet<HomeOwner> HomeOwners { get; set; }
    }
}
