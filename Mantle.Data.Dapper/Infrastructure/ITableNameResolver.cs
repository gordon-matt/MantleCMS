namespace Mantle.Data.Dapper.Infrastructure;

public interface ITableNameResolver
{
    string GetTableName(Type entityType);
}