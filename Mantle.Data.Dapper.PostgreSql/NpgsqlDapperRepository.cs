using System;
using System.Collections.Generic;
using System.Data;
using Extenso.Data.Entity;
using Extenso.Data.QueryBuilder;
using Mantle.Data.PostgreSql;
using Npgsql;

namespace Mantle.Data.Dapper.PostgreSql
{
    public class NpgsqlDapperRepository<TEntity, TKey> : DapperRepository<TEntity, TKey>
        where TEntity : BaseEntity<TKey>
    {
        public NpgsqlDapperRepository(string connectionString, string tableName, string schema = null)
            : base(connectionString, tableName, schema)
        {
        }

        protected override string LastInsertedRowCommand
        {
            get { return @"RETURNING ""Id"""; }
        }

        public override IDbConnection OpenConnection()
        {
            var connection = new NpgsqlConnection(ConnectionString);
            connection.Open();
            return connection;
        }

        public override IEnumerable<TEntity> Find(ISelectQueryBuilder queryBuilder)
        {
            if (!(queryBuilder is PostgreSqlSelectQueryBuilder))
            {
                throw new ArgumentException("queryBuilder must be of type, 'PostgreSqlSelectQueryBuilder'", "queryBuilder");
            }
            return base.Find(queryBuilder);
        }

        public override int Count(ISelectQueryBuilder queryBuilder)
        {
            if (!(queryBuilder is PostgreSqlSelectQueryBuilder))
            {
                throw new ArgumentException("queryBuilder must be of type, 'PostgreSqlSelectQueryBuilder'", "queryBuilder");
            }
            return base.Count(queryBuilder);
        }

        public override ISelectQueryBuilder CreateQuery()
        {
            return new PostgreSqlSelectQueryBuilder(schema);
        }

        protected override string EncloseIdentifier(string identifier)
        {
            return string.Concat('"', identifier, '"');
        }
    }
}