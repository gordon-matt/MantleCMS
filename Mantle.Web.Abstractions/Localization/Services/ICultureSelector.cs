namespace Mantle.Web.Localization.Services;

public interface ICultureSelector
{
    CultureSelectorResult GetCulture(HttpContext context);
}