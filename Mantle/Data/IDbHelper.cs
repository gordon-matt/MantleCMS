using System.Data.Common;

namespace Mantle.Data
{
    public interface IDbHelper
    {
        string Escape(string s);

        bool CheckIfTableExists(DbConnection connection, string tableName);

        DbConnection CreateConnection(string connectionString);
    }
}