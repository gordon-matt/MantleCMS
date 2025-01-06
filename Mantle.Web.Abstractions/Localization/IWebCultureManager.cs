using Mantle.Localization;

namespace Mantle.Web.Localization;

public interface IWebCultureManager : ICultureManager
{
    string GetCurrentCulture(HttpContext httpContext);
}