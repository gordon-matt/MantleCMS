using Autofac;
using Autofac.Core;

namespace Mantle.Data.Dapper.Infrastructure;

public class TableNameParameter : Parameter
{
    public override bool CanSupplyValue(ParameterInfo pi, IComponentContext context, out Func<object> valueProvider)
    {
        valueProvider = null;

        if (pi.ParameterType != typeof(string) || pi.Name != "tableName")
        {
            return false;
        }

        valueProvider = () =>
        {
            var entityType = pi.Member.DeclaringType.GetGenericArguments()[0];

            var tableNameResolver = context.Resolve<ITableNameResolver>();
            string tableName = tableNameResolver.GetTableName(entityType);
            return tableName;
        };

        return true;
    }
}