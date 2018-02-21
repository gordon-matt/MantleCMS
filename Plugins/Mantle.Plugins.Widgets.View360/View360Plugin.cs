﻿using Mantle.Plugins.Widgets.View360.Infrastructure;
using Mantle.Web.Plugins;

namespace Mantle.Plugins.Widgets.View360
{
    public class View360Plugin : BasePlugin
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