using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFDDD.DataModel.Migrations.AzureImportExport.Internal
{
    internal static class Extensions
    {
        internal static string ToSecretConnectionString(this string connectionString, string passwordHider = "******")
        {
            connectionString = connectionString ?? string.Empty;
            var sqlconnBuilder = new SqlConnectionStringBuilder
            {
                ConnectionString = connectionString,
                Password = passwordHider
            };
            return sqlconnBuilder.ToString();
        }
    }
}
