using Mantle.Localization;
using Mantle.Web.Localization.Services;

namespace Mantle.Web.Localization;

public class WebCultureManager : DefaultCultureManager, IWebCultureManager
{
    private readonly IEnumerable<ICultureSelector> cultureSelectors;

    public WebCultureManager(IEnumerable<ICultureSelector> cultureSelectors)
    {
        this.cultureSelectors = cultureSelectors;
    }

    #region IWebCultureManager Members

    public virtual string GetCurrentCulture(HttpContext httpContext)
    {
        var requestedCultures = cultureSelectors
            .Select(x => x.GetCulture(httpContext))
            .ToList()
            .Where(x => x != null)
            .OrderByDescending(x => x.Priority);

        string cultureCode = null;

        if (requestedCultures.Any())
        {
            cultureCode = requestedCultures.First().CultureCode;
        }

        if (cultureCode == string.Empty)
        {
            cultureCode = null;
        }

        return cultureCode;
    }

    #endregion IWebCultureManager Members
}