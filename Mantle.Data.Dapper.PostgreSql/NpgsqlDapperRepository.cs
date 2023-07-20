using Extenso.Data.Entity;
using Extenso.Data.QueryBuilder;
using Extenso.Data.QueryBuilder.Npgsql;
using Npgsql;
using System.Data;

namespace Mantle.Data.Dapper.PostgreSql
{
    public class NpgsqlDapperRepository<TEntity, TKey> : DapperRepository<TEntity, TKey>
        where TEntity : BaseEntity<TKey>
    {
        public NpgsqlDapperRepository(string connectionString, string tableName, string schema = null)
            : base(connectionString, tableName, schema)
        {
        }

        protected override string LastInsertedRowCommand => @"RETURNING ""Id""";

        public override IDbConnection OpenConnection()
        {
            var connection = new NpgsqlConnection(ConnectionString);
            connection.Open();
            return connection;
        }

        public override IEnumerable<TEntity> Find(ISelectQueryBuilder queryBuilder)
        {
            if (!(queryBuilder is NpgsqlSelectQueryBuilder))
            {
                throw new ArgumentException("queryBuilder must be of type, 'NpgsqlSelectQueryBuilder'", "queryBuilder");
            }
            return base.Find(queryBuilder);
        }

        public override int Count(ISelectQueryBuilder queryBuilder)
        {
            if (!(queryBuilder is NpgsqlSelectQueryBuilder))
            {
                throw new ArgumentException("queryBuilder must be of type, 'NpgsqlSelectQueryBuilder'", "queryBuilder");
            }
            return base.Count(queryBuilder);
        }

        public override ISelectQueryBuilder CreateQuery()
        {
            return new NpgsqlSelectQueryBuilder(schema);
        }

        protected override string EncloseIdentifier(string identifier)
        {
            return $"\"{identifier}\"";
        }
    }
}