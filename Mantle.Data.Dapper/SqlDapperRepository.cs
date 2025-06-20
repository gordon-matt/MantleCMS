﻿using Microsoft.Data.SqlClient;

namespace Mantle.Data.Dapper;

public class SqlDapperRepository<TEntity, TKey> : DapperRepository<TEntity, TKey>
    where TEntity : BaseEntity<TKey>
{
    public SqlDapperRepository(string connectionString, string tableName, string schema = null)
        : base(connectionString, tableName, schema)
    {
    }

    protected override string LastInsertedRowCommand => "SELECT CAST(SCOPE_IDENTITY() AS int)";

    public override IDbConnection OpenConnection()
    {
        var connection = new SqlConnection(ConnectionString);
        connection.Open();
        return connection;
    }

    public override IEnumerable<TEntity> Find(ISelectQueryBuilder queryBuilder) => queryBuilder is not SqlServerSelectQueryBuilder
        ? throw new ArgumentException("queryBuilder must be of type, 'SqlServerSelectQueryBuilder'", nameof(queryBuilder))
        : base.Find(queryBuilder);

    public override int Count(ISelectQueryBuilder queryBuilder) => queryBuilder is not SqlServerSelectQueryBuilder
        ? throw new ArgumentException("queryBuilder must be of type, 'SqlServerSelectQueryBuilder'", nameof(queryBuilder))
        : base.Count(queryBuilder);

    public override ISelectQueryBuilder CreateQuery() => new SqlServerSelectQueryBuilder();

    protected override string EncloseIdentifier(string identifier) => $"[{identifier}]";
}