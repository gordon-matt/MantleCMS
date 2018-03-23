using Mantle.Plugins.Widgets.FlexSlider.Infrastructure;
using Mantle.Plugins;

namespace Mantle.Plugins.Widgets.FlexSlider
{
    public class FlexSliderPlugin : BasePlugin
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