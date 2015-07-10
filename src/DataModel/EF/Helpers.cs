using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDDD.DataModel.EF
{
    public static class Helpers
    {
        /// <summary>
        /// Sets Initializer to null. Use overload to state otherwise
        /// </summary>
        public static void SetInitializer()
        {
            SetInitializer<Context>(null);
        }

        public static void SetInitializer<TDbContext>(IDatabaseInitializer<TDbContext> databaseInitializer) where TDbContext : DbContext, new()
        {
            try
            {
                Database.SetInitializer(databaseInitializer);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("The Commerce Context could not initialize.", ex);
            }
        }

        public static string GetRandomConnectionString(string databaseName = null,
            string connectionTemplate =
                @"Data Source=(LocalDb)\v11.0;Initial Catalog={{DATABASE_NAME}};Integrated Security=SSPI;")
        {
            databaseName = databaseName ?? string.Format("ContextData_{0}", Guid.NewGuid().ToString("N"));
            var connectionString = connectionTemplate.Replace("{{DATABASE_NAME}}", databaseName);

            return connectionString;
        }

        public static bool IsIdSetToDefault<TEntity>(TEntity entity, string entityIdName)
        {
            var id = FindEntityIdValue(entity, entityIdName);

            if (id is int)
            {
                return (int)id == default(int);
            }

            if (id is Guid)
            {
                return (Guid)id == default(Guid);
            }

            if (id is string)
            {
                return (string)id == null;
            }

            throw new NotImplementedException("DataEntity Id only allowed to be of types: int, Guid, string");
        }

        public static string FindIdName<TEntity>()
        {
            string entityIdName = null;
            var classIdName = typeof(TEntity).Name + "Id";

            var properties = typeof(TEntity).GetProperties();
            foreach (var property in properties)
            {
                if (property.Name.ToLower() == classIdName.ToLower())
                {
                    entityIdName = property.Name;
                    continue;
                }

                if (property.Name.ToLower() == "id")
                {
                    entityIdName = property.Name;
                    continue;
                }
            }

            return entityIdName;
        }

        public static object FindEntityIdValue<TEntity>(TEntity entity, string entityIdName)
        {
            var idPropertyValue = entity.GetType().GetProperty(entityIdName).GetValue(entity, null);
            return idPropertyValue;
        }

        public static bool IsUsingInterface<TInterface, TEntity>()
        {
            return typeof(TEntity).GetInterface(typeof(TInterface).FullName) != null;
        }
    }
}
