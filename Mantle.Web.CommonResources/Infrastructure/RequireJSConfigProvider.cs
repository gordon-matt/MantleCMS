using Mantle.Web.Infrastructure;

namespace Mantle.Web.CommonResources.Infrastructure;

public class RequireJSConfigProvider : IRequireJSConfigProvider
{
    #region IRequireJSConfigProvider Members

    public IDictionary<string, string> Paths => new Dictionary<string, string>
    {
        { "mantle-common", "/_content/MantleFramework.Web.CommonResources/js/mantle-common" },
        { "mantle-knockout-chosen", "/_content/MantleFramework.Web.CommonResources/js/mantle-knockout-chosen" },
        { "mantle-section-switching", "/_content/MantleFramework.Web.CommonResources/js/mantle-section-switching" },
        { "mantle-tinymce", "/_content/MantleFramework.Web.CommonResources/js/tinymce/mantle-tinymce" },
        { "momentjs", "/_content/MantleFramework.Web.CommonResources/js/moment-with-locales.min" },
        { "grid-helper", "/_content/MantleFramework.Web.CommonResources/js/grid-helper" },
        { "odata-helpers", "/_content/MantleFramework.Web.CommonResources/js/odata-helpers" },
        { "mantle-toasts", "/mantle/common/scripts/toasts" }
    };

    public IDictionary<string, string[]> Shim => new Dictionary<string, string[]>
    {
        { "mantle-knockout-chosen", new[] { "chosen", "knockout" } },
        { "mantle-tinymce", new[] { "tinymce" } },
        { "odata-helpers", new[] { "mantle-translations" } }
    };

    #endregion IRequireJSConfigProvider Members
}