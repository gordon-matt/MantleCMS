namespace Mantle.Web.Localization;

public class MantleStringLocalizerFactory : IStringLocalizerFactory
{
    private MantleStringLocalizer stringLocalizer;

    public IStringLocalizer Create(Type resourceSource) => Create();

    public IStringLocalizer Create(string baseName, string location) => Create();

    protected IStringLocalizer Create()
    {
        if (stringLocalizer == null)
        {
            var cacheManager = EngineContext.Current.Resolve<ICacheManager>();
            var localizableStringService = EngineContext.Current.Resolve<ILocalizableStringService>();
            var workContext = EngineContext.Current.Resolve<IWorkContext>();
            stringLocalizer = new MantleStringLocalizer(cacheManager, workContext, localizableStringService);
        }
        return stringLocalizer;
    }
}