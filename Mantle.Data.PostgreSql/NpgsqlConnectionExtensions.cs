using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using Npgsql;
using NpgsqlTypes;

namespace Mantle.Data.PostgreSql
{
    public static class NpgsqlConnectionExtensions
    {
        public static IEnumerable<string> GetDatabaseNames(this NpgsqlConnection connection)
        {
            const string CMD_SELECT_DATABASE_NAMES = "SELECT datname FROM pg_database;";
            var databaseNames = new List<string>();

            bool alreadyOpen = (connection.State != ConnectionState.Closed);

            if (!alreadyOpen)
            {
                connection.Open();
            }

            using (var command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = CMD_SELECT_DATABASE_NAMES;

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        databaseNames.Add(reader.GetString(0));
                    }
                }
            }

            if (!alreadyOpen)
            {
                connection.Close();
            }

            return databaseNames;
        }

        public static IEnumerable<string> GetTableNames(this NpgsqlConnection connection, bool includeViews = false)
        {
            string query = string.Empty;

            if (includeViews)
            {
                query = @"SELECT table_name
FROM information_schema.tables
WHERE table_schema = 'dbo'
ORDER BY table_name";
            }
            else
            {
                query = @"SELECT table_name
FROM information_schema.tables
WHERE table_schema = 'dbo'
AND table_type = 'BASE TABLE'
ORDER BY table_name";
            }

            var tables = new List<string>();

            bool alreadyOpen = (connection.State != ConnectionState.Closed);

            if (!alreadyOpen)
            {
                connection.Open();
            }

            using (var command = connection.CreateCommand())
            {
                command.CommandType = CommandType.Text;
                command.CommandText = query;

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        tables.Add(reader.GetString(0));
                    }
                }
            }

            if (!alreadyOpen)
            {
                connection.Close();
            }

            return tables;
        }

        public static ForeignKeyInfoCollection GetForeignKeyData(this NpgsqlConnection connection, string tableName)
        {
            const string CMD_FOREIGN_KEYS_FORMAT =
@"SELECT
	FK.""table_name"" AS FK_Table,

    CU.""column_name"" AS FK_Column,
    PK.""table_name"" AS PK_Table,
    PT.""column_name"" AS PK_Column,
    C.""constraint_name"" AS Constraint_Name
FROM information_schema.""referential_constraints"" C
INNER JOIN information_schema.""table_constraints"" FK ON C.""constraint_name"" = FK.""constraint_name""
INNER JOIN information_schema.""table_constraints"" PK ON C.""unique_constraint_name"" = PK.""constraint_name""
INNER JOIN information_schema.""key_column_usage"" CU ON C.""constraint_name"" = CU.""constraint_name""
INNER JOIN
(
    SELECT i1.""table_name"", i2.""column_name""
    FROM information_schema.""table_constraints"" i1
    INNER JOIN information_schema.""key_column_usage"" i2 ON i1.""constraint_name"" = i2.""constraint_name""
    WHERE i1.""constraint_type"" = 'PRIMARY KEY'
) PT ON PT.""table_name"" = PK.""table_name""
WHERE FK.""table_name"" = '{0}'
ORDER BY 1,2,3,4";

            ForeignKeyInfoCollection foreignKeyData = new ForeignKeyInfoCollection();

            bool alreadyOpen = (connection.State != ConnectionState.Closed);

            if (!alreadyOpen)
            {
                connection.Open();
            }

            using (var command = new NpgsqlCommand(string.Format(CMD_FOREIGN_KEYS_FORMAT, tableName), connection))
            {
                command.CommandType = CommandType.Text;
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        foreignKeyData.Add(new ForeignKeyInfo(
                            reader.GetString(0),
                            reader.GetString(1),
                            reader.GetString(2),
                            reader.GetString(3),
                            string.Empty,
                            reader.GetString(4)));
                    }
                }
            }

            if (!alreadyOpen)
            {
                connection.Close();
            }

            return foreignKeyData;
        }

        public static ColumnInfoCollection GetColumnData(this NpgsqlConnection connection, string tableName)
        {
            const string CMD_COLUMN_INFO_FORMAT =
@"SELECT ""column_name"", ""column_default"", ""data_type"", ""character_maximum_length"", ""is_nullable""
FROM information_schema.""columns""
WHERE TABLE_NAME = '{0}';";

            const string CMD_IS_PRIMARY_KEY_FORMAT =
@"SELECT ""column_name""
FROM information_schema.""key_column_usage"" kcu
INNER JOIN information_schema.""table_constraints"" tc ON kcu.""constraint_name"" = tc.""constraint_name""
WHERE kcu.""table_name"" = '{0}'
AND tc.""constraint_type"" = 'PRIMARY KEY'";

            var list = new ColumnInfoCollection();

            bool alreadyOpen = (connection.State != ConnectionState.Closed);

            try
            {
                var foreignKeyColumns = connection.GetForeignKeyData(tableName);

                if (!alreadyOpen)
                {
                    connection.Open();
                }

                using (var command = new NpgsqlCommand(string.Format(CMD_COLUMN_INFO_FORMAT, tableName), connection))
                {
                    command.CommandType = CommandType.Text;
                    using (var reader = command.ExecuteReader())
                    {
                        ColumnInfo columnInfo = null;

                        while (reader.Read())
                        {
                            columnInfo = new ColumnInfo();

                            if (!reader.IsDBNull(0))
                            { columnInfo.ColumnName = reader.GetString(0); }

                            if (!reader.IsDBNull(1))
                            { columnInfo.DefaultValue = reader.GetString(1); }
                            else
                            { columnInfo.DefaultValue = string.Empty; }

                            if (foreignKeyColumns.Contains(columnInfo.ColumnName))
                            {
                                columnInfo.KeyType = KeyType.ForeignKey;
                            }

                            //else
                            //{
                            try
                            {
                                // TODO: SqlDbType won't work for PG!! Need to update this ASAP... get System.Type from PG type
                                string type = reader.GetString(2).ToLowerInvariant();
                                switch (type)
                                {
                                    case "bigint": columnInfo.DataType = typeof(long); break;
                                    case "bigserial": columnInfo.DataType = typeof(long); break;
                                    case "bit": columnInfo.DataType = typeof(bool); break;
                                    case "bit varying": columnInfo.DataType = typeof(BitArray); break;
                                    case "boolean": columnInfo.DataType = typeof(bool); break;
                                    case "box": columnInfo.DataType = typeof(NpgsqlBox); break;
                                    case "bytea": columnInfo.DataType = typeof(byte[]); break;
                                    case "character": columnInfo.DataType = typeof(string); break;
                                    case "character varying": columnInfo.DataType = typeof(string); break;
                                    case "cidr": columnInfo.DataType = typeof(NpgsqlInet); break;
                                    case "circle": columnInfo.DataType = typeof(NpgsqlCircle); break;
                                    case "date": columnInfo.DataType = typeof(DateTime); break;
                                    case "double precision": columnInfo.DataType = typeof(double); break;
                                    case "inet": columnInfo.DataType = typeof(IPAddress); break;
                                    case "integer": columnInfo.DataType = typeof(int); break;
                                    case "interval": columnInfo.DataType = typeof(TimeSpan); break;
                                    case "json": columnInfo.DataType = typeof(string); break;
                                    case "line": columnInfo.DataType = typeof(NpgsqlLine); break;
                                    case "lseg": columnInfo.DataType = typeof(NpgsqlLSeg); break;
                                    case "macaddr": columnInfo.DataType = typeof(PhysicalAddress); break;
                                    case "money": columnInfo.DataType = typeof(decimal); break;
                                    case "numeric": columnInfo.DataType = typeof(decimal); break;
                                    case "path": columnInfo.DataType = typeof(NpgsqlPath); break;
                                    case "point": columnInfo.DataType = typeof(NpgsqlPoint); break;
                                    case "polygon": columnInfo.DataType = typeof(NpgsqlPolygon); break;
                                    case "real": columnInfo.DataType = typeof(float); break;
                                    case "smallint": columnInfo.DataType = typeof(short); break;
                                    case "smallserial": columnInfo.DataType = typeof(short); break;
                                    case "serial": columnInfo.DataType = typeof(int); break;
                                    case "text": columnInfo.DataType = typeof(string); break;
                                    case "time without time zone": columnInfo.DataType = typeof(TimeSpan); break;
                                    case "time with time zone": columnInfo.DataType = typeof(DateTimeOffset); break;
                                    case "timestamp without time zone": columnInfo.DataType = typeof(DateTime); break;
                                    case "timestamp with time zone": columnInfo.DataType = typeof(DateTime); break;
                                    case "tsquery": columnInfo.DataType = typeof(NpgsqlTsQuery); break;
                                    case "tsvector": columnInfo.DataType = typeof(NpgsqlTsVector); break;
                                    case "txid_snapshot": columnInfo.DataType = typeof(object); break;
                                    case "uuid": columnInfo.DataType = typeof(Guid); break;
                                    case "xml": columnInfo.DataType = typeof(string); break;
                                    default: columnInfo.DataType = typeof(object); break;
                                }
                            }
                            catch (ArgumentNullException)
                            {
                                columnInfo.DataType = typeof(object);
                            }
                            catch (ArgumentException)
                            {
                                columnInfo.DataType = typeof(object);
                            }

                            //}

                            if (!reader.IsDBNull(3))
                            { columnInfo.MaximumLength = reader.GetInt32(3); }

                            if (!reader.IsDBNull(4))
                            {
                                if (reader.GetString(4).ToUpperInvariant().Equals("NO"))
                                { columnInfo.IsNullable = false; }
                                else
                                { columnInfo.IsNullable = true; }
                            }

                            list.Add(columnInfo);
                        }
                    }
                }
            }
            finally
            {
                if (!alreadyOpen && connection.State != ConnectionState.Closed)
                { connection.Close(); }
            }

            #region Primary Keys

            using (var command = connection.CreateCommand())
            {
                command.CommandText = string.Format(CMD_IS_PRIMARY_KEY_FORMAT, tableName);

                alreadyOpen = (connection.State != ConnectionState.Closed);

                if (!alreadyOpen)
                {
                    connection.Open();
                }

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string pkColumn = reader.GetString(0);
                        ColumnInfo match = list[pkColumn];
                        if (match != null)
                        {
                            match.KeyType = KeyType.PrimaryKey;
                        }
                    }
                }

                if (!alreadyOpen)
                {
                    connection.Close();
                }
            }

            #endregion Primary Keys

            return list;
        }

        public static ColumnInfoCollection GetColumnData(this NpgsqlConnection connection, string tableName, IEnumerable<string> columnNames)
        {
            var data = from x in connection.GetColumnData(tableName)
                       where columnNames.Contains(x.ColumnName)
                       select x;

            ColumnInfoCollection collection = new ColumnInfoCollection();
            collection.AddRange(data);
            return collection;
        }
    }
}