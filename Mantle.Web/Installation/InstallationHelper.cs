using System;
using System.Data;
using System.Data.SqlClient;
using Extenso.Data.Common;
using Extenso.Data.Entity;
using Mantle.Data.Entity;
using Mantle.Infrastructure;
using Mantle.Web.Models;
using Microsoft.EntityFrameworkCore;

namespace Mantle.Web.Installation
{
    public static class InstallationHelper
    {
        /// <summary>
        /// {0}: Server, {1}: Database, {2}: User, {3}: Password
        /// </summary>
        private static string ConnectionStringFormat = @"Server={0};Initial Catalog={1};User={2};Password={3};Persist Security Info=True;MultipleActiveResultSets=True";

        /// <summary>
        /// {0}: Server, {1}: Database
        /// </summary>
        private static string ConnectionStringWAFormat = @"Server={0};Initial Catalog={1};Integrated Security=True;Persist Security Info=True;MultipleActiveResultSets=True";

        public static void Install<TContext>(InstallationModel model) where TContext : DbContext, ISupportSeed
        {
            var dataSettings = EngineContext.Current.Resolve<DataSettings>();

            string connectionString;
            if (model.EnterConnectionString)
            {
                connectionString = model.ConnectionString;
            }
            else
            {
                if (model.UseWindowsAuthentication)
                {
                    connectionString = string.Format(
                        ConnectionStringWAFormat,
                        model.DatabaseServer,
                        model.DatabaseName);
                }
                else
                {
                    connectionString = string.Format(
                        ConnectionStringFormat,
                        model.DatabaseServer,
                        model.DatabaseName,
                        model.DatabaseUsername,
                        model.DatabasePassword);
                }
            }

            dataSettings.ConnectionString = connectionString;

            // We need to save the Password to settings temporarily in order to setup the login details AFTER restarting the app domain
            //  We then delete the password from the XML file in Mantle.Web.Infrastructure.StartupTask.
            dataSettings.AdminEmail = model.AdminEmail;
            dataSettings.AdminPassword = model.AdminPassword;
            dataSettings.CreateSampleData = model.CreateSampleData;

            DataSettingsManager.SaveSettings(dataSettings);

            var contextFactory = EngineContext.Current.Resolve<IDbContextFactory>();
            using (var context = contextFactory.GetContext(connectionString))
            {
                context.Database.Migrate(); // TODO: Test
                ((TContext)context).Seed();
            }

            //using (var connection = new SqlConnection(connectionString))
            //{
            //    bool exists = CheckDatabaseExists(connection, model.DatabaseName);

            //    int numberOfTables = 0;

            //    if (exists)
            //    {
            //        numberOfTables = connection.ExecuteScalar<int>(
            //            "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE table_type = 'BASE TABLE'");
            //    }

            //    if (!exists || numberOfTables == 0)
            //    {
            //        string creationScript;

            //        using (var context = new TContext())
            //        {
            //            creationScript = context.Database.GenerateCreateScript();
            //        }

            //        using (var command = connection.CreateCommand())
            //        {
            //            command.CommandType = CommandType.Text;
            //            command.CommandText = creationScript;
            //            command.ExecuteNonQuery();
            //        }
            //    }
            //}

            //using (var context = new TContext())
            //{
            //    var connection = context.Database.GetDbConnection();
            //    connection.ConnectionString = connectionString;

            //    bool dbExists = context.Database.GetService<IRelationalDatabaseCreator>().Exists();
            //    if (dbExists)
            //    {
            //        int numberOfTables = context.Database.SqlQuery<int>(
            //            "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE table_type = 'BASE TABLE'").FirstAsync().Result;

            //        if (numberOfTables == 0)
            //        {
            //            var dbCreationScript = context.Database.GenerateCreateScript();
            //            context.Database.ExecuteSqlRaw(dbCreationScript);
            //        }
            //    }
            //    else
            //    {
            //        context.Database.Migrate(); // TODO: Test
            //    }

            //    context.Seed();
            //}

            DataSettingsHelper.ResetCache();

            var webHelper = EngineContext.Current.Resolve<IWebHelper>();
            webHelper.RestartAppDomain();
        }

        //private static bool CheckDatabaseExists(SqlConnection connection, string databaseName)
        //{
        //    bool exists;

        //    try
        //    {
        //        connection.Open();
        //        int databaseId = connection.ExecuteScalar<int>($"SELECT database_id FROM sys.databases WHERE Name = '{databaseName}'");
        //        connection.Close();
        //        exists = (databaseId > 0);
        //    }
        //    catch
        //    {
        //        exists = false;
        //    }

        //    return exists;
        //}
    }
}