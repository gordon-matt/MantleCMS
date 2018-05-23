using Extenso.Data.Entity;
using Mantle.Data.Entity;
using Mantle.Infrastructure;
using Mantle.Plugins.Widgets.FullCalendar.Infrastructure;

namespace Mantle.Plugins.Widgets.FullCalendar
{
    public class FullCalendarPlugin : BasePlugin
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

            //DropTable(dbContext, Constants.Tables.Events);
            //DropTable(dbContext, Constants.Tables.Calendars);

            base.Uninstall();
        }
    }
}