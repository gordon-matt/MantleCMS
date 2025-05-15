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
            var cacheManager = DependoResolver.Instance.Resolve<ICacheManager>();
            var localizableStringService = DependoResolver.Instance.Resolve<ILocalizableStringService>();
            var workContext = DependoResolver.Instance.Resolve<IWorkContext>();
            stringLocalizer = new MantleStringLocalizer(cacheManager, workContext, localizableStringService);
        }
        return stringLocalizer;
    }
}