using System;
using System.Linq;
using Mantle.Data.Entity.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace Mantle.Data.PostgreSql
{
    public class PostgreSqlEntityFrameworkHelper : IEntityFrameworkHelper
    {
        //public bool SupportsBulkInsert
        //{
        //    get { return false; }
        //}

        //public bool SupportsEFExtended
        //{
        //    get { return false; }
        //}

        public void EnsureTables<TContext>(TContext context) where TContext : DbContext
        {
            throw new NotImplementedException();

            //            if (!context.Database.Exists())
            //            {
            //                string databaseToCreate = context.Database.Connection.Database;

            //                string connectionString = context.Database.Connection.ConnectionString.Replace(
            //                    "Database=" + databaseToCreate,
            //                    "Database=postgres");

            //                using (var connection = new NpgsqlConnection(connectionString))
            //                using (var command = connection.CreateCommand())
            //                {
            //                    command.CommandType = CommandType.Text;
            //                    command.CommandText = string.Format(
            //                        @"CREATE DATABASE ""{0}"" WITH OWNER = ""postgres"" ENCODING = 'UTF8' CONNECTION LIMIT = -1;",
            //                        databaseToCreate);
            //                    connection.Open();
            //                    command.ExecuteNonQuery();
            //                    connection.Close();
            //                }

            //                using (var connection = new NpgsqlConnection(context.Database.Connection.ConnectionString))
            //                using (var command = connection.CreateCommand())
            //                {
            //                    command.CommandType = CommandType.Text;
            //                    command.CommandText = @"CREATE SCHEMA ""dbo"";";
            //                    connection.Open();
            //                    command.ExecuteNonQuery();
            //                    connection.Close();
            //                }
            //            }

            //            var objectContext = ((IObjectContextAdapter)context).ObjectContext;
            //            var metadata = objectContext.MetadataWorkspace;
            //            var sSpaceEntityTypes = metadata.GetItemCollection(DataSpace.SSpace).OfType<EntityType>();

            //            var cSpaceItems = metadata.GetItemCollection(DataSpace.CSpace)
            //                .Where(x =>
            //                    x.GetType() == typeof(AssociationType) ||
            //                    x.GetType() == typeof(EntityType));

            //            var tableNames = new HashSet<string>();
            //            var tableScripts = new HashSet<string>();
            //            var otherScripts = new HashSet<string>();
            //            foreach (var entityType in cSpaceItems.OfType<EntityType>())
            //            {
            //                #region Normal Table Definitions

            //                var sSpaceEntityType = sSpaceEntityTypes.FirstOrDefault(x => x.Name == entityType.Name);

            //                var fields = new HashSet<string>();
            //                bool isMultiKey = entityType.KeyProperties.Count > 1;
            //                var keys = new List<string>();

            //                // We need to use a "for" loop, not a "foreach" loop, so we can use the index for...
            //                for (int i = 0; i < entityType.DeclaredProperties.Count; i++)
            //                {
            //                    // doing this:
            //                    //  Luckily, the columns in the CSpace and SSPace entity types are in the exact same order,
            //                    //      so, we can use that to get the correct column name. This is needed in case someone uses ".HasColumnName(name)"
            //                    //      in the EntityTypeConfiguraton<T> class.
            //                    //      The CSpace EdmProperty will contain the default property name, example: Surname
            //                    //      While the SSpace EdmProperty will contain the actual name for the DB, example: FamilyName
            //                    var property = entityType.DeclaredProperties[i];
            //                    var sSpaceProperty = sSpaceEntityType.DeclaredProperties[i];

            //                    if (!isMultiKey)
            //                    {
            //                        bool isPrimaryKey = entityType.KeyProperties.Contains(property.Name);
            //                        string field = GetFieldDefinition(property, sSpaceProperty.Name, isPrimaryKey);
            //                        fields.Add(field);
            //                    }
            //                    else
            //                    {
            //                        keys.Add(sSpaceProperty.Name);
            //                        string field = GetFieldDefinition(property, sSpaceProperty.Name, false);//pass false here and we do keys after
            //                        fields.Add(field);
            //                    }
            //                }

            //                if (isMultiKey)
            //                {
            //                    fields.Add(string.Format("PRIMARY KEY ({0})", string.Join("\",\"", keys).AddDoubleQuotes()));
            //                }

            //                var tableName = sSpaceEntityType.MetadataProperties["TableName"].Value ?? sSpaceEntityType.Name;
            //                string sql = string.Format("CREATE TABLE IF NOT EXISTS dbo.\"{0}\" ({1});", tableName, string.Join(", ", fields));
            //                tableNames.Add(tableName.ToString());
            //                tableScripts.Add(sql);

            //                #region Navigation Properties

            //                var navigationProperties = entityType.Members.Where(x => x.BuiltInTypeKind == BuiltInTypeKind.NavigationProperty);

            //                foreach (NavigationProperty navProperty in navigationProperties)
            //                {
            //                    var navPropertyToEndMemberElementType = ((RefType)navProperty.ToEndMember.TypeUsage.EdmType).ElementType;
            //                    var sSpaceNavPropertyToEndMemberEntityType = sSpaceEntityTypes.FirstOrDefault(x => x.Name == navPropertyToEndMemberElementType.Name);
            //                    var sSpaceNavPropertyToEndMemberTableName = sSpaceNavPropertyToEndMemberEntityType.MetadataProperties["TableName"].Value ?? sSpaceNavPropertyToEndMemberEntityType.Name;

            //                    var dependantProperties = navProperty.GetDependentProperties();
            //                    if (!dependantProperties.IsNullOrEmpty())
            //                    {
            //                        foreach (var property in dependantProperties)
            //                        {
            //                            string fk = string.Format(
            //@"ALTER TABLE dbo.""{0}"" DROP CONSTRAINT IF EXISTS ""{1}_fkey"";
            //ALTER TABLE dbo.""{0}"" ADD CONSTRAINT ""{1}_fkey"" FOREIGN KEY (""{1}"") REFERENCES dbo.""{2}"";",
            //                                tableName,
            //                                property.Name,
            //                                sSpaceNavPropertyToEndMemberTableName);

            //                            otherScripts.Add(fk);
            //                        }
            //                    }
            //                }

            //                #endregion Navigation Properties

            //                #endregion Normal Table Definitions
            //            }

            //            #region Junction Tables

            //            var junctionTables = new Dictionary<string, AssociationType>();
            //            foreach (AssociationType associationType in cSpaceItems.OfType<AssociationType>())
            //            {
            //                if (associationType.IsForeignKey)
            //                {
            //                    // We don't want to deal with foreign keys here, only with junction tables (many-to-many).
            //                    continue;
            //                }

            //                if (associationType.KeyMembers
            //                    .OfType<AssociationEndMember>()
            //                    .Any(x => x.RelationshipMultiplicity != RelationshipMultiplicity.Many))
            //                {
            //                    // We don't want to deal with foreign keys here, only with junction tables (many-to-many).
            //                    continue;
            //                }

            //                string tableName = associationType.Name;
            //                if (!tableNames.Contains(tableName))
            //                {
            //                    junctionTables.Add(tableName, associationType);
            //                }
            //            }

            //            foreach (var table in junctionTables)
            //            {
            //                var query1 = table.Value.KeyMembers
            //                    .Where(x => x.TypeUsage.EdmType.GetType() == typeof(RefType))
            //                    .Select(x => ((RefType)x.TypeUsage.EdmType).ElementType)
            //                    .ToDictionary(k => k.Name, v => v);

            //                var sSpaceEntityType = sSpaceEntityTypes.FirstOrDefault(x => x.Name.ContainsAll(query1.Keys.ToArray()));

            //                if (sSpaceEntityType != null)
            //                {
            //                    var junctionTable = new StringBuilder(128);
            //                    var tableName = sSpaceEntityType.MetadataProperties["TableName"].Value ?? sSpaceEntityType.Name;
            //                    junctionTable.AppendFormat("CREATE TABLE IF NOT EXISTS dbo.\"{0}\" (", tableName);

            //                    var keys = new List<string>();
            //                    foreach (EdmProperty property in sSpaceEntityType.DeclaredMembers)
            //                    {
            //                        string fieldName = property.Name;
            //                        string dataType = property.TypeName;

            //                        string refFieldName = fieldName.Replace("_Id", string.Empty);
            //                        refFieldName = refFieldName.Replace("Id", string.Empty);
            //                        if (!query1.ContainsKey(refFieldName))
            //                        {
            //                            break;
            //                        }

            //                        string cSpaceRefTableName = query1[refFieldName].Name; // ??
            //                        var sSpaceRefTable = sSpaceEntityTypes.FirstOrDefault(x => x.Name == cSpaceRefTableName);

            //                        if (sSpaceRefTable == null)
            //                        {
            //                            break;
            //                        }

            //                        // TODO: Update & Delete Cascades, if they're set
            //                        // Also: We are currently hard coding the foreign key in parent table to the name "Id".
            //                        //  This should be fixed, if possible. Not Important for now, because 99% of entities will have primary key named "Id"
            //                        keys.Add(fieldName);
            //                        var refTableName = sSpaceRefTable.MetadataProperties["TableName"].Value ?? sSpaceRefTable.Name;
            //                        junctionTable.AppendFormat("\"{0}\" {1} REFERENCES dbo.\"{2}\"(\"Id\"),", fieldName, dataType, refTableName);
            //                    }

            //                    junctionTable.AppendFormat("CONSTRAINT \"{0}_pkey\" PRIMARY KEY ({1}));", table.Key, string.Join("\",\"", keys).AddDoubleQuotes());
            //                    tableScripts.Add(junctionTable.ToString());
            //                }
            //            }

            //            #endregion Junction Tables

            //            if (tableScripts.Count > 0)
            //            {
            //                var connection = context.Database.Connection;
            //                var isConnectionClosed = connection.State == ConnectionState.Closed;

            //                if (isConnectionClosed)
            //                {
            //                    connection.Open();
            //                }

            //                try
            //                {
            //                    var createCommand = connection.CreateCommand();
            //                    createCommand.CommandText = string.Join("", tableScripts);
            //                    createCommand.ExecuteNonQuery();

            //                    if (otherScripts.Count > 0)
            //                    {
            //                        createCommand.CommandText = string.Join("", otherScripts);
            //                        createCommand.ExecuteNonQuery();
            //                    }
            //                }
            //                catch (DbException)
            //                {
            //                    // Ignore when have database exception
            //                    //Logger.Error(ex.Message, ex);
            //                }
            //                catch (EntityException)
            //                {
            //                    // Ignore when have database exception
            //                }
            //                finally
            //                {
            //                    if (isConnectionClosed)
            //                    {
            //                        connection.Close();
            //                    }
            //                }
            //            }
        }

        //private string GetFieldDefinition(EdmProperty property, string fieldName, bool isPrimaryKey)
        //{
        //    var field = new StringBuilder(128);
        //    string dataType = GetDataTypeForCSpaceProperty(property);

        //    if (isPrimaryKey)
        //    {
        //        if (dataType.In("SMALLINT", "INTEGER"))
        //        {
        //            dataType = "SERIAL";
        //        }
        //        else if (dataType == "BIGINT")
        //        {
        //            dataType = "BIGSERIAL";
        //        }
        //    }

        //    field.Append("\"");
        //    field.Append(fieldName);
        //    field.Append("\" ");
        //    field.Append(dataType);

        //    if (!property.Nullable)
        //    {
        //        field.Append(" NOT NULL");
        //    }

        //    if (property.DefaultValue != null)
        //    {
        //        field.Append(" DEFAULT ");
        //        field.Append(property.DefaultValue);
        //    }

        //    if (isPrimaryKey)
        //    {
        //        field.Append(" PRIMARY KEY");
        //    }

        //    return field.ToString();
        //}

        //private string GetDataTypeForCSpaceProperty(EdmProperty property)
        //{
        //    switch (property.TypeName)
        //    {
        //        case "String":
        //        case "StringFixedLength":
        //        case "AnsiString":
        //        case "AnsiStringFixedLength":
        //            return property.MaxLength.HasValue
        //                ? (
        //                    property.IsFixedLength.HasValue && property.IsFixedLength.Value
        //                        ? "CHARACTER(" + property.MaxLength.Value + ")"
        //                        : "CHARACTER VARYING(" + property.MaxLength.Value + ")"
        //                  )
        //                : "TEXT";

        //        case "Binary":
        //        case "Object": return "BYTEA";
        //        case "Boolean": return "BOOLEAN";
        //        case "Byte":
        //        case "SByte":
        //        case "Int16":
        //        case "UInt16": return "SMALLINT";
        //        case "Currency": return "MONEY";
        //        case "Date": return "DATE";
        //        case "DateTime":
        //        case "DateTime2": return "TIMESTAMP";
        //        case "DateTimeOffset": return "TIMESTAMP WITH TIME ZONE";
        //        case "Decimal":
        //        case "VarNumeric":
        //            {
        //                if (property.Precision.HasValue && property.Scale.HasValue)
        //                {
        //                    return string.Format("NUMERIC({0},{1})", property.Precision.Value, property.Scale.Value);
        //                }
        //                return "NUMERIC";
        //            }
        //        case "Double": return "DOUBLE PRECISION";
        //        case "Guid": return "UUID";
        //        case "Int32":
        //        case "UInt32": return "INTEGER";
        //        case "Int64":
        //        case "UInt64": return "BIGINT";
        //        case "Single": return "REAL";
        //        case "Time": return "TIME WITHOUT TIME ZONE";
        //        case "Xml": return "XML";

        //        default:
        //            if (property.IsEnumType)
        //            {
        //                string dataType = string.Empty;
        //                switch (property.UnderlyingPrimitiveType.ToString())
        //                {
        //                    case "Edm.Int32": return "INTEGER";
        //                    case "Edm.Int64": return "BIGINT";
        //                    default: return "SMALLINT";
        //                }
        //            }
        //            else
        //            {
        //                if (!property.IsPrimitiveType)
        //                {
        //                    throw new NotSupportedException(string.Format("Cannot support {0} type.", property.TypeName));
        //                }
        //                throw new ArgumentOutOfRangeException();
        //            }
        //    }
        //}
    }
}