using System.Data.SqlClient;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dapper;

namespace DataModel.Migrations.Test
{
    internal static class InternalHelpers
    {
        internal static async Task CreateDatabase(string connectionString, string databaseName)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                await conn.ExecuteAsync(string.Format("CREATE DATABASE [{0}]", databaseName));
            }

            byte state;
            do
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    await conn.OpenAsync();
                    var results = await conn.QueryAsync<byte>("SELECT [state] FROM sys.databases WHERE name = @databaseName", new { databaseName });
                    state = results.First();
                    if (state != 0)
                    {
                        Thread.Sleep(3000);
                    }
                }
            }
            while (state != 0);
        }

        internal static async Task DropDatabase(string connectionString, string databaseName)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                await conn.OpenAsync();
                await conn.ExecuteAsync(string.Format("DROP DATABASE [{0}]", databaseName));
            }
        }
    }
}