namespace Mantle.Web.Localization;

public class CurrentCultureCodeStateProvider : IWorkContextStateProvider
{
    private readonly ICacheManager cacheManager;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly IWebCultureManager cultureManager;

    public CurrentCultureCodeStateProvider(
        ICacheManager cacheManager,
        IHttpContextAccessor httpContextAccessor,
        IWebCultureManager cultureManager)
    {
        this.cacheManager = cacheManager;
        this.cultureManager = cultureManager;
        this.httpContextAccessor = httpContextAccessor;
    }

    #region IWorkContextStateProvider Members

    public Func<IWorkContext, T> Get<T>(string name) => name == MantleWebConstants.StateProviders.CurrentCultureCode
        ? (ctx => cacheManager.Get(MantleWebConstants.CacheKeys.CurrentCulture, () => (T)(object)cultureManager.GetCurrentCulture(httpContextAccessor.HttpContext)))
        : null;

    #endregion IWorkContextStateProvider Members
}