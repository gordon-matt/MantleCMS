//using System;
//using System.Collections.Generic;
//using System.Data;
//using System.Data.Common;
//using System.Data.Entity;
//using System.Data.Entity.Core.Objects;
//using System.Data.Entity.Infrastructure;
//using System.Text.RegularExpressions;
//using Mantle.Data.Common;
//using Microsoft.EntityFrameworkCore;

//namespace Mantle.Data.Entity.EntityFramework
//{
//    public static class DbContextExtensions
//    {
//        public static T ExecuteScalar<T>(this DbContext dbContext, string queryText)
//        {
//            return dbContext.Database.Connection.ExecuteScalar<T>(queryText);
//        }

//        public static DataSet ExecuteStoredProcedure(this DbContext dbContext, string storedProcedure, IEnumerable<DbParameter> parameters)
//        {
//            return dbContext.Database.Connection.ExecuteStoredProcedure(storedProcedure, parameters);
//        }

//        public static DataSet ExecuteStoredProcedure(this DbContext dbContext, string storedProcedure, IEnumerable<DbParameter> parameters, out Dictionary<string, object> outputValues)
//        {
//            return dbContext.Database.Connection.ExecuteStoredProcedure(storedProcedure, parameters, out outputValues);
//        }

//        public static int ExecuteNonQueryStoredProcedure(this DbContext dbContext, string storedProcedure, IEnumerable<DbParameter> parameters)
//        {
//            return dbContext.Database.Connection.ExecuteNonQueryStoredProcedure(storedProcedure, parameters);
//        }

//        public static DbParameter CreateParameter(this DbContext dbContext, string parameterName, object value)
//        {
//            return dbContext.Database.Connection.CreateParameter(parameterName, value);
//        }

//        public static System.Data.Entity.Core.EntityKey GetEntityKey<T>(this DbContext context, T entity) where T : class
//        {
//            var oc = ((IObjectContextAdapter)context).ObjectContext;
//            ObjectStateEntry ose;
//            if (null != entity && oc.ObjectStateManager.TryGetObjectStateEntry(entity, out ose))
//            {
//                return ose.EntityKey;
//            }
//            return null;
//        }

//        public static string GetEntitySetName<T>(this DbContext dbContext) where T : class
//        {
//            var set = dbContext.Set<T>();
//            var regex = new Regex("FROM (?<table>.*) AS");
//            string sql = set.ToString();
//            var match = regex.Match(sql);

//            return match.Groups["table"].Value;
//        }

//        public static string GetEntitySetName(this DbContext dbContext, Type entityType)
//        {
//            //TODO: Test which way is faster: this (Regex) or below (using ObjectContext)...

//            var set = dbContext.Set(entityType);
//            var regex = new Regex("FROM (?<table>.*) AS");
//            string sql = set.ToString();
//            var match = regex.Match(sql);

//            return match.Groups["table"].Value;
//        }
//    }
//}