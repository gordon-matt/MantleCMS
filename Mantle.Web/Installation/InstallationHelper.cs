//using Mantle.Infrastructure;
//using Mantle.Web.Models;
//using Microsoft.EntityFrameworkCore;

//namespace Mantle.Web.Installation
//{
//    public static class InstallationHelper
//    {
//        /// <summary>
//        /// {0}: Server, {1}: Database, {2}: User, {3}: Password
//        /// </summary>
//        private static string ConnectionStringFormat = @"Server={0};Initial Catalog={1};User={2};Password={3};Persist Security Info=True;MultipleActiveResultSets=True";

//        /// <summary>
//        /// {0}: Server, {1}: Database
//        /// </summary>
//        private static string ConnectionStringWAFormat = @"Server={0};Initial Catalog={1};Integrated Security=True;Persist Security Info=True;MultipleActiveResultSets=True";

//        public static void Install<TContext>(InstallationModel model) where TContext : DbContext, IKoreDbContext, ISupportSeed, new()
//        {
//            var dataSettings = EngineContext.Current.Resolve<DataSettings>();

//            string connectionString = string.Empty;

//            if (model.EnterConnectionString)
//            {
//                connectionString = model.ConnectionString;
//            }
//            else
//            {
//                if (model.UseWindowsAuthentication)
//                {
//                    connectionString = string.Format(
//                        ConnectionStringWAFormat,
//                        model.DatabaseServer,
//                        model.DatabaseName);
//                }
//                else
//                {
//                    connectionString = string.Format(
//                        ConnectionStringFormat,
//                        model.DatabaseServer,
//                        model.DatabaseName,
//                        model.DatabaseUsername,
//                        model.DatabasePassword);
//                }
//            }

//            dataSettings.ConnectionString = connectionString;

//            // We need to save the Password to settings temporarily in order to setup the login details AFTER restarting the app domain
//            //  We then delete the password from the XML file in Kore.Web.Infrastructure.StartupTask.
//            dataSettings.AdminEmail = model.AdminEmail;
//            dataSettings.AdminPassword = model.AdminPassword;
//            dataSettings.CreateSampleData = model.CreateSampleData;

//            DataSettingsManager.SaveSettings(dataSettings);

//            using (var context = new TContext())
//            {
//                context.Database.Connection.ConnectionString = connectionString;

//                bool dbExists = context.Database.Exists();
//                if (dbExists)
//                {
//                    int numberOfTables = context.Database.SqlQuery<int>(
//                        "SELECT COUNT(*) FROM INFORMATION_SCHEMA.TABLES WHERE table_type = 'BASE TABLE'").FirstAsync().Result;

//                    if (numberOfTables == 0)
//                    {
//                        var dbCreationScript = ((IObjectContextAdapter)context).ObjectContext.CreateDatabaseScript();
//                        context.Database.ExecuteSqlCommand(dbCreationScript);
//                    }
//                }
//                else
//                {
//                    context.Database.Create();
//                }
//                context.Seed();
//            }

//            DataSettingsHelper.ResetCache();

//            var webHelper = EngineContext.Current.Resolve<IWebHelper>();
//            webHelper.RestartAppDomain();
//        }
//    }
//}