namespace Mantle.Data.Dapper;

public abstract class DapperRepository<TEntity, TKey> : IDapperRepository<TEntity, TKey>
    where TEntity : BaseEntity<TKey>
{
    protected readonly string schema = null;
    protected readonly string tableName = null;

    public DapperRepository(string connectionString, string tableName, string schema = null)
    {
        ConnectionString = connectionString;
        this.tableName = tableName;
        this.schema = schema;
    }

    protected string ConnectionString { get; private set; }

    protected abstract string LastInsertedRowCommand { get; }

    #region IDapperRepository<TEntity, TKey> Members

    public abstract IDbConnection OpenConnection();

    public virtual IEnumerable<TEntity> Find()
    {
        using (var connection = OpenConnection())
        {
            return connection.Query<TEntity>(string.Concat("SELECT * FROM ", GetTableName()));
        }
    }

    public virtual IEnumerable<TEntity> Find(ISelectQueryBuilder queryBuilder)
    {
        string sql = queryBuilder.BuildQuery();
        using (var connection = OpenConnection())
        {
            return connection.Query<TEntity>(sql);
        }
    }

    public virtual IEnumerable<TEntity> Find(string predicate, int skip = 0, short take = -1, string orderBy = null, SortDirection sortDirection = SortDirection.Ascending)
    {
        using (var connection = OpenConnection())
        {
            if (take > 0)
            {
                return connection.Query<TEntity>(string.Format(
                    "SELECT * FROM {0} WHERE {1} ORDER BY {2} OFFSET {3} ROWS FETCH NEXT {4} ROWS ONLY;",
                    GetTableName(),
                    predicate,
                    orderBy == null ? "1 " + GetDirection(sortDirection) : EncloseIdentifier(orderBy) + " " + GetDirection(sortDirection),
                    skip,
                    take));
            }

            return connection.Query<TEntity>(string.Format(
                "SELECT * FROM {0} WHERE {1}{2}",
                GetTableName(),
                predicate,
                orderBy == null ? null : "ORDER BY " + EncloseIdentifier(orderBy) + " " + GetDirection(sortDirection)));
        }
    }

    public virtual TEntity FindOne(TKey id)
    {
        using (var connection = OpenConnection())
        {
            return connection
                .Query<TEntity>(string.Format(
                    "SELECT * FROM {0} WHERE {1} = {2}",
                    GetTableName(),
                    EncloseIdentifier("Id"),
                    FormatValue(id)))
                .FirstOrDefault();
        }
    }

    public virtual TEntity FindOne(string predicate)
    {
        return Find(predicate, 0, 1).FirstOrDefault();
    }

    public virtual int Count()
    {
        using (var connection = OpenConnection())
        {
            return connection.Query<int>(string.Concat("SELECT COUNT(*) FROM ", GetTableName())).FirstOrDefault();
        }
    }

    public virtual int Count(ISelectQueryBuilder queryBuilder)
    {
        string sql = queryBuilder.BuildQuery();
        using (var connection = OpenConnection())
        {
            return connection.Query<int>(sql).FirstOrDefault();
        }
    }

    public virtual int Count(string predicate)
    {
        using (var connection = OpenConnection())
        {
            return connection
                .Query<int>(string.Format(
                    "SELECT COUNT(*) FROM {0} WHERE {1}",
                    GetTableName(),
                    predicate))
                .FirstOrDefault();
        }
    }

    public virtual int Delete(TEntity entity)
    {
        int recordsAffected = 0;
        using (var connection = OpenConnection())
        {
            try
            {
                string sql = string.Format("DELETE FROM {0} WHERE {1} = @Id;", GetTableName(), EncloseIdentifier("Id"));
                recordsAffected = connection.Execute(sql, entity);
            }
            catch
            {
                return 0;
            }

            return recordsAffected;
        }
    }

    public virtual int Delete(IEnumerable<TEntity> entities)
    {
        int recordsAffected = 0;
        using (var connection = OpenConnection())
        {
            try
            {
                var ids = string.Join(",", entities.Select(x => FormatValue(x.Id)));
                string sql = string.Format("DELETE FROM {0} WHERE {1} IN({2});", GetTableName(), EncloseIdentifier("Id"), ids);
                recordsAffected = connection.Execute(sql);
            }
            catch
            {
                return 0;
            }

            return recordsAffected;
        }
    }

    public virtual bool Insert(TEntity entity)
    {
        var propertyContainer = ParseProperties(entity);
        string sql = string.Format("INSERT INTO {0}({1}) VALUES(@{2}) {3}",
            GetTableName(),
            string.Join(", ", propertyContainer.ValueNames.Select(x => EncloseIdentifier(x))),
            string.Join(", @", propertyContainer.ValueNames),
            LastInsertedRowCommand);

        using (var connection = OpenConnection())
        {
            try
            {
                var propertyInfo = entity.GetType().GetTypeInfo().GetProperty("Id");

                switch (Type.GetTypeCode(propertyInfo.PropertyType))
                {
                    case TypeCode.Int32:
                        {
                            int newId = connection.Query<int>(sql, entity).FirstOrDefault();
                            propertyInfo.SetValue(entity, newId);
                        }
                        break;

                    case TypeCode.Int64:
                        {
                            long newId = connection.Query<long>(sql, entity).FirstOrDefault();
                            propertyInfo.SetValue(entity, newId);
                        }
                        break;

                    case TypeCode.Int16:
                        {
                            short newId = connection.Query<short>(sql, entity).FirstOrDefault();
                            propertyInfo.SetValue(entity, newId);
                        }
                        break;

                    case TypeCode.Byte:
                        {
                            byte newId = connection.Query<byte>(sql, entity).FirstOrDefault();
                            propertyInfo.SetValue(entity, newId);
                        }
                        break;

                    default: connection.Execute(sql, entity); break;
                }
            }
            catch
            {
                return false;
            }

            return true;
        }
    }

    public virtual int Insert(IEnumerable<TEntity> entities)
    {
        int rowsAffected = 0;

        using (var connection = OpenConnection())
        {
            try
            {
                foreach (var entity in entities)
                {
                    var propertyContainer = ParseProperties(entity);
                    string sql = string.Format("INSERT INTO {0}({1}) VALUES(@{2})",
                        GetTableName(),
                        string.Join(", ", propertyContainer.ValueNames.Select(x => EncloseIdentifier(x))),
                        string.Join(", @", propertyContainer.ValueNames));

                    rowsAffected += connection.Execute(sql, entity);
                }
            }
            catch
            {
                return 0;
            }

            return rowsAffected;
        }
    }

    public virtual int Update(TEntity entity)
    {
        int recordsAffected = 0;
        using (var connection = OpenConnection())
        {
            try
            {
                var propertyContainer = ParseProperties(entity);
                var sqlIdPairs = GetSqlPairs(propertyContainer.IdNames);
                var sqlValuePairs = GetSqlPairs(propertyContainer.ValueNames);
                var sql = string.Format("UPDATE {0} SET {1} WHERE {2}", GetTableName(), sqlValuePairs, sqlIdPairs);
                recordsAffected = connection.Execute(sql, entity);
            }
            catch
            {
                return 0;
            }

            return recordsAffected;
        }
    }

    public virtual int Update(IEnumerable<TEntity> entities)
    {
        int recordsAffected = 0;
        using (var connection = OpenConnection())
        {
            try
            {
                foreach (var entity in entities)
                {
                    var propertyContainer = ParseProperties(entity);
                    var sqlIdPairs = GetSqlPairs(propertyContainer.IdNames);
                    var sqlValuePairs = GetSqlPairs(propertyContainer.ValueNames);
                    var sql = string.Format("UPDATE {0} SET {1} WHERE {2}", GetTableName(), sqlValuePairs, sqlIdPairs);
                    recordsAffected += connection.Execute(sql, entity);
                }
            }
            catch
            {
                return 0;
            }

            return recordsAffected;
        }
    }

    public abstract ISelectQueryBuilder CreateQuery();

    #endregion IDapperRepository<TEntity, TKey> Members

    protected virtual string GetTableName()
    {
        if (!string.IsNullOrEmpty(schema))
        {
            return string.Concat(schema, '.', EncloseIdentifier(tableName));
        }
        return EncloseIdentifier(tableName);
    }

    protected abstract string EncloseIdentifier(string identifier);

    protected virtual object FormatValue(object value)
    {
        if (value is string || value is Guid || value is DateTime)
        {
            return string.Concat("'", value, "'");
        }
        return value;
    }

    //TODO: Cache the properties (without values, of course)
    protected static PropertyContainer ParseProperties(TEntity entity)
    {
        var propertyContainer = new PropertyContainer();

        var type = typeof(TEntity);

        var typeName = type.Name;
        var validKeyNames = new[]
        {
            "Id",
            string.Concat(typeName, "Id"),
            string.Concat(typeName,"_Id")
        };

        var properties = type.GetProperties();
        foreach (var property in properties)
        {
            // Skip reference types (but still include string!)
            if (property.PropertyType.GetTypeInfo().IsClass && property.PropertyType != typeof(string))
            {
                continue;
            }

            // Skip methods without a public setter
            if (property.GetSetMethod() == null)
            {
                continue;
            }

            // Skip methods specifically ignored
            if (property.IsDefined(typeof(DapperIgnore), false))
            {
                continue;
            }

            var name = property.Name;
            var value = type.GetProperty(property.Name).GetValue(entity, null);

            if (property.IsDefined(typeof(DapperKey), false) || validKeyNames.Contains(name))
            {
                propertyContainer.AddId(name, value);
            }
            else
            {
                propertyContainer.AddValue(name, value);
            }
        }

        return propertyContainer;
    }

    protected string GetSqlPairs(IEnumerable<string> keys, string separator = ",")
    {
        var pairs = keys.Select(key => string.Format("{0}=@{1}", EncloseIdentifier(key), key)).ToList();
        return string.Join(separator, pairs);
    }

    protected string GetDirection(SortDirection sortDirection)
    {
        return sortDirection == SortDirection.Descending ? "DESC" : "ASC";
    }

    #region Nested Types

    protected class PropertyContainer
    {
        private readonly Dictionary<string, object> _ids;
        private readonly Dictionary<string, object> _values;

        #region Properties

        internal IEnumerable<string> IdNames
        {
            get { return _ids.Keys; }
        }

        internal IEnumerable<string> ValueNames
        {
            get { return _values.Keys; }
        }

        internal IEnumerable<string> AllNames
        {
            get { return _ids.Keys.Union(_values.Keys); }
        }

        internal IDictionary<string, object> IdPairs
        {
            get { return _ids; }
        }

        internal IDictionary<string, object> ValuePairs
        {
            get { return _values; }
        }

        internal IEnumerable<KeyValuePair<string, object>> AllPairs
        {
            get { return _ids.Concat(_values); }
        }

        #endregion Properties

        #region Constructor

        internal PropertyContainer()
        {
            _ids = new Dictionary<string, object>();
            _values = new Dictionary<string, object>();
        }

        #endregion Constructor

        #region Methods

        internal void AddId(string name, object value)
        {
            _ids.Add(name, value);
        }

        internal void AddValue(string name, object value)
        {
            _values.Add(name, value);
        }

        #endregion Methods
    }

    #endregion Nested Types
}