using Mantle.Exceptions;
using Mantle.Web.Mvc.Assets;

namespace Mantle.Web
{
    public class MantleWebAssets
    {
        private static bool isInitialized = false;
        private static MantleWebAssets instance = null;

        public static MantleWebAssets Instance
        {
            get
            {
                if (instance == null || !isInitialized)
                {
                    throw new MantleException(string.Format("Web assets for {0} have not been initialized.", nameof(MantleWebAssets)));
                }
                return instance;
            }
            private set { instance = value; }
        }

        public static void Init(MantleWebAssets assets)
        {
            Instance = assets;
            isInitialized = true;
        }

        public AssetCollection BootstrapFileInput { get; set; }
    }
}