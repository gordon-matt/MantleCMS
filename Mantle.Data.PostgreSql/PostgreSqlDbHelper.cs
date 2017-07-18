using System;
using System.Data.Common;
using System.Linq;
using Npgsql;

namespace Mantle.Data.PostgreSql
{
    public class PostgreSqlDbHelper : IDbHelper
    {
        public string Escape(string s)
        {
            return string.Concat('\"', s, '\"');
        }

        public bool CheckIfTableExists(DbConnection connection, string tableName)
        {
            if (!(connection is NpgsqlConnection))
            {
                throw new ArgumentException("Specified connection is not an NpgsqlConnection.", "connection");
            }

            var pgSqlonnection = connection as NpgsqlConnection;
            return pgSqlonnection.GetTableNames().Contains(tableName);
        }

        public DbConnection CreateConnection(string connectionString)
        {
            return new NpgsqlConnection(connectionString);
        }
    }
}