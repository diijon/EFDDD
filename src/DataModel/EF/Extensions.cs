using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Infrastructure.Pluralization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDDD.DataModel.EF
{
    public static class Extensions
    {
        public static string GetTableName<T>(this IContext context)
            where T : class
        {
            var type = typeof(T);
            var pluralizationService = new EnglishPluralizationService();
            var entityName = pluralizationService.Pluralize(type.Name);

            var tableAttribute = type.GetCustomAttributes(false).OfType<System.ComponentModel.DataAnnotations.Schema.TableAttribute>().FirstOrDefault();

            return string.Format("{0}.{1}", Constants.DB_SCHEMA, (tableAttribute == null ? entityName : tableAttribute.Name));
        }

        public static bool DoesIdExist<T>(this IContext context, string entityIdName, object id)
            where T : class
        {
            var tableName = GetTableName<T>(context);

            var sql = string.Format("SELECT CAST(1 as bit) FROM {0} WHERE {1} = @p0", tableName, entityIdName);
            var exists = context.Database.SqlQuery<bool>(sql, id);
            return exists.FirstOrDefault();
        }

        public static IContext ClearChangeTracker(this IContext _this)
        {
            foreach (var entry in _this.ChangeTracker.Entries())
            {
                entry.State = EntityState.Detached;
            }
            return _this;
        }

        public static IContext ClearChangeTracker<T>(this IContext _this) where T : class
        {
            foreach (var entry in _this.ChangeTracker.Entries<T>())
            {
                entry.State = EntityState.Detached;
            }
            return _this;
        }

        public static IContext UndoChangeTracker(this IContext _this)
        {
            foreach (var entry in _this.ChangeTracker.Entries())
            {
                if (entry.State == EntityState.Modified)
                {
                    entry.State = EntityState.Unchanged;
                }
                else if (entry.State == EntityState.Added)
                {
                    entry.State = EntityState.Detached;
                }
            }
            return _this;
        }

        public static IContext UndoChangeTracker<T>(this IContext _this) where T : class
        {
            foreach (var entry in _this.ChangeTracker.Entries<T>())
            {
                if (entry.State == EntityState.Modified)
                {
                    entry.State = EntityState.Unchanged;
                }
                else if (entry.State == EntityState.Added)
                {
                    entry.State = EntityState.Detached;
                }
            }
            return _this;
        }

        public static IEnumerable<DbEntityEntry<T>> GetTrackedEntities<T>(this IContext _this) where T : class
        {
            return _this.ChangeTracker.Entries<T>();
        }

        public static IEnumerable<DbEntityEntry> GetTrackedEntities(this IContext _this)
        {
            return _this.ChangeTracker.Entries();
        }
    }


}
