using Mantle.Exceptions;

namespace Mantle.Web.ContentManagement
{
    public class MantleCmsAssets
    {
        private static bool isInitialized = false;
        private static MantleCmsAssets instance = null;

        public static MantleCmsAssets Instance
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

        public static void Init(MantleCmsAssets assets)
        {
            Instance = assets;
            isInitialized = true;
        }

        //public AssetCollection ElFinder { get; set; }
    }
}