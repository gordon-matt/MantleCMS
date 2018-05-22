using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Extenso.Data.Entity;
using Extenso.Data.QueryBuilder;

namespace Mantle.Data.Dapper
{
    public class SqlDapperRepository<TEntity, TKey> : DapperRepository<TEntity, TKey>
        where TEntity : BaseEntity<TKey>
    {
        public SqlDapperRepository(string connectionString, string tableName, string schema = null)
            : base(connectionString, tableName, schema)
        {
        }

        protected override string LastInsertedRowCommand
        {
            get { return "SELECT CAST(SCOPE_IDENTITY() AS int)"; }
        }

        public override IDbConnection OpenConnection()
        {
            var connection = new SqlConnection(ConnectionString);
            connection.Open();
            return connection;
        }

        public override IEnumerable<TEntity> Find(ISelectQueryBuilder queryBuilder)
        {
            if (!(queryBuilder is SqlServerSelectQueryBuilder))
            {
                throw new ArgumentException("queryBuilder must be of type, 'SqlServerSelectQueryBuilder'", "queryBuilder");
            }
            return base.Find(queryBuilder);
        }

        public override int Count(ISelectQueryBuilder queryBuilder)
        {
            if (!(queryBuilder is SqlServerSelectQueryBuilder))
            {
                throw new ArgumentException("queryBuilder must be of type, 'SqlServerSelectQueryBuilder'", "queryBuilder");
            }
            return base.Count(queryBuilder);
        }

        public override ISelectQueryBuilder CreateQuery()
        {
            return new SqlServerSelectQueryBuilder();
        }

        protected override string EncloseIdentifier(string identifier)
        {
            return string.Concat('[', identifier, ']');
        }
    }
}