using Mantle.Web.Mvc.Assets;

namespace Mantle.Web.Messaging;

public class MantleMessagingAssets
{
    private static bool isInitialized = false;
    private static MantleMessagingAssets instance = null;

    public static MantleMessagingAssets Instance
    {
        get
        {
            if (instance == null || !isInitialized)
            {
                throw new MantleException(string.Format("Web assets for {0} have not been initialized.", nameof(MantleMessagingAssets)));
            }
            return instance;
        }
        private set { instance = value; }
    }

    public static void Init(MantleMessagingAssets assets)
    {
        Instance = assets;
        isInitialized = true;
    }

    public AssetCollection GrapesJs { get; set; }

    public AssetCollection GrapesJsMjml { get; set; }

    public AssetCollection GrapesJsAviary { get; set; }
}