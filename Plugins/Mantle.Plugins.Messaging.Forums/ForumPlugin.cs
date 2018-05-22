﻿using Extenso.Data.Entity;
using Mantle.Data.Entity;
using Mantle.Infrastructure;
using Mantle.Plugins.Messaging.Forums.Infrastructure;

namespace Mantle.Plugins.Messaging.Forums
{
    public class ForumPlugin : BasePlugin
    {
        public override void Install()
        {
            base.Install();
            InstallLanguagePack<LanguagePackInvariant>();
        }

        public override void Uninstall()
        {
            UninstallLanguagePack<LanguagePackInvariant>();

            var dbContextFactory = EngineContext.Current.Resolve<IDbContextFactory>();
            var dbContext = dbContextFactory.GetContext();

            //TODO:
            //DropTable(dbContext, Constants.Tables.PrivateMessages);
            //DropTable(dbContext, Constants.Tables.Subscriptions);
            //DropTable(dbContext, Constants.Tables.Posts);
            //DropTable(dbContext, Constants.Tables.Topics);
            //DropTable(dbContext, Constants.Tables.Forums);
            //DropTable(dbContext, Constants.Tables.Groups);

            base.Uninstall();
        }
    }
}