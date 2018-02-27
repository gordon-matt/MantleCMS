using System.Collections.Generic;
using Mantle.Web.Mvc.MantleUI.Providers;

namespace Mantle.Web.Mvc.MantleUI
{
    public static class MantleUISettings
    {
        private static IMantleUIProvider defaultAdminProvider;
        private static IMantleUIProvider defaultFrontendProvider;

        static MantleUISettings()
        {
            AreaUIProviders = new Dictionary<string, IMantleUIProvider>();
        }

        public static Dictionary<string, IMantleUIProvider> AreaUIProviders { get; private set; }

        public static IMantleUIProvider DefaultAdminProvider
        {
            get { return defaultAdminProvider ?? (defaultAdminProvider = new Bootstrap3UIProvider()); }
            set { defaultAdminProvider = value; }
        }

        public static IMantleUIProvider DefaultFrontendProvider
        {
            get { return defaultFrontendProvider ?? (defaultFrontendProvider = new Bootstrap3UIProvider()); }
            set { defaultFrontendProvider = value; }
        }

        public static void RegisterAreaUIProvider(string area, IMantleUIProvider provider)
        {
            if (!AreaUIProviders.ContainsKey(area))
            {
                AreaUIProviders.Add(area, provider);
            }
        }
    }
}