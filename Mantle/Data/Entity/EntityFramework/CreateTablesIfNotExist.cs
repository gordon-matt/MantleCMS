//using System;
//using System.Data.Entity;
//using System.Transactions;
//using Mantle.Infrastructure;
//using Mantle.Logging;
//using Microsoft.EntityFrameworkCore;

//namespace Mantle.Data.Entity.EntityFramework
//{
//    public class CreateTablesIfNotExist<TContext> : IDatabaseInitializer<TContext> where TContext : DbContext
//    {
//        public void InitializeDatabase(TContext context)
//        {
//            bool dbExists;

//            try
//            {
//                dbExists = context.Database.Exists();
//            }
//            catch
//            {
//                dbExists = false;
//            }

//            if (dbExists)
//            {
//                var databaseInitializerHelper = EngineContext.Current.Resolve<IMantleEntityFrameworkHelper>();
//                databaseInitializerHelper.EnsureTables(context);
//            }
//            else
//            {
//                //please don't remove this.. if you want it to work, then add "Persist Security Info=true" to your connection string.
//                try
//                {
//                    var defaultInitializer = new CreateDatabaseIfNotExists<TContext>();
//                    defaultInitializer.InitializeDatabase(context);
//                }
//                catch (Exception x)
//                {
//                    var logger = LoggingUtilities.Resolve();
//                    logger.Error(x.Message, x);
//                }
//            }
//        }
//    }
//}