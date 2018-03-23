using Mantle.Plugins.Widgets.JQueryFormBuilder.Infrastructure;
using Mantle.Plugins;

namespace Mantle.Plugins.Widgets.JQueryFormBuilder
{
    public class JQueryFormBuilderPlugin : BasePlugin
    {
        public override void Install()
        {
            base.Install();
            InstallLanguagePack<LanguagePackInvariant>();
        }

        public override void Uninstall()
        {
            base.Uninstall();
            UninstallLanguagePack<LanguagePackInvariant>();
        }
    }
}