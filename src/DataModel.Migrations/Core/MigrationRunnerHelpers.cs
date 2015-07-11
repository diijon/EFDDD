using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Threading;

namespace EFDDD.DataModel.Migrations.Core
{
    internal static class MigrationRunnerHelpers
    {
        internal static IEnumerable<string> GetTargetDatabaseNames(string connectionString)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    const string sql = "SELECT [name] FROM sys.databases WHERE [name] NOT IN('master','tempdb','model','msdb') ORDER BY [name]";
                    using (var reader = SqlRead(cmd, sql))
                    {
                        if (!reader.HasRows)
                        {
                            return new List<string>();
                        }

                        var dbNames = new List<string>();
                        while (reader.Read())
                        {
                            dbNames.Add(reader[0].ToString());
                        }
                        return dbNames;
                    }
                }
            }
        }

        internal static bool ContainsMigration(string connectionString, string dbSchema, string migrationName)
        {
            var hasMigrationTable = ContainsMigrationTable(connectionString, dbSchema);
            if (!hasMigrationTable) { return false; }

            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    var sql = string.Format("select TOP 1 MigrationId from [{0}].[__MigrationHistory] where MigrationId = '{1}' order by MigrationId DESC", dbSchema, migrationName);
                    using (var reader = SqlRead(cmd, sql))
                    {
                        return reader.HasRows;
                    }
                }
            }
        }

        internal static bool ContainsMigrationTable(string connectionString, string dbSchema)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    var sql = string.Format("SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = '{0}' AND  TABLE_NAME = '{1}'", dbSchema, "__MigrationHistory");
                    using (var reader = SqlRead(cmd, sql))
                    {
                        return reader.HasRows;
                    }
                }
            }
        }

        internal static void RenameDatabase(string oldDatabaseName, string newDatabaseName, string connectionString)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        SqlCmd(cmd, string.Format("ALTER DATABASE [{0}] MODIFY NAME = [{1}]", oldDatabaseName, newDatabaseName));
                    }
                }
                catch { throw; }
                finally
                {
                    if (conn.State == ConnectionState.Open) { conn.Close(); }
                    Thread.Sleep(2000);
                }
            }

            byte state = 0;
            do
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = string.Format("SELECT [state] FROM sys.databases WHERE name='{0}'", newDatabaseName);
                            state = (byte)cmd.ExecuteScalar();
                        }
                    }
                    catch { throw; }
                    finally
                    {
                        if (conn.State == ConnectionState.Open) { conn.Close(); }
                        if (state != 0)
                        {
                            Console.WriteLine("still waiting on database renaming of {0} to {1} to complete...", oldDatabaseName, newDatabaseName);
                            Thread.Sleep(3500);
                        }
                    }
                }
            }
            while (state != 0);
        }

        internal static void DeleteDatabase(string connectionString, string databaseName)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    SqlCmd(cmd, string.Format("DROP DATABASE [{0}]", databaseName));
                }
            }
        }

        internal static bool IsServerAzure(string connectionString)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    const string sql = "SELECT @@VERSION AS [Version]";
                    using (var reader = SqlRead(cmd, sql))
                    {
                        if (!reader.HasRows)
                        {
                            return false;
                        }

                        reader.Read();
                        var dbVersion = reader[0].ToString();
                        return Regex.IsMatch(dbVersion, @"(Azure)");
                    }
                }
            }
        }

        internal static void CreateDatabase(string connectionString, string databaseName)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    using (var cmd = conn.CreateCommand())
                    {
                        SqlCmd(cmd, string.Format("CREATE DATABASE [{0}]", databaseName));
                    }
                }
                catch { throw; }
                finally
                {
                    if (conn.State == ConnectionState.Open) { conn.Close(); }
                    Thread.Sleep(2000);
                }
            }

            byte state = 0;
            do
            {
                using (var conn = new SqlConnection(connectionString))
                {
                    try
                    {
                        conn.Open();
                        using (var cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = string.Format("SELECT [state] FROM sys.databases WHERE name='{0}'", databaseName);
                            state = (byte)cmd.ExecuteScalar();
                        }
                    }
                    catch { throw; }
                    finally
                    {
                        if (conn.State == ConnectionState.Open) { conn.Close(); }
                        if (state != 0)
                        {
                            Console.WriteLine("still waiting on database creation of {0} to complete...", databaseName);
                            Thread.Sleep(3500);
                        }
                    }
                }
            }
            while (state != 0);
        }

        internal static bool DoesDatabaseExist(string connectionString, string databaseName)
        {
            using (var conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    var sql = string.Format("SELECT [name] FROM sys.databases WHERE [name] NOT IN('master','tempdb','model','msdb') AND [name] = '{0}'", databaseName);
                    using (var reader = SqlRead(cmd, sql))
                    {
                        return reader.HasRows;
                    }
                }
            }
        }

        internal static readonly Action<SqlCommand, string> SqlCmd = (cmd, sql) =>
        {
            cmd.CommandText = sql;
            cmd.CommandTimeout = 30;
            cmd.ExecuteNonQuery();
        };
        internal static readonly Func<SqlCommand, string, SqlDataReader> SqlRead = (cmd, sql) =>
        {
            cmd.CommandText = sql;
            cmd.CommandTimeout = 30;
            return cmd.ExecuteReader();
        };
    }
}