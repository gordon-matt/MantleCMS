using Mantle.Web.Infrastructure;

namespace Mantle.Web.CommonResources.Infrastructure;

public class RequireJSConfigProvider : IRequireJSConfigProvider
{
    #region IRequireJSConfigProvider Members

    public IDictionary<string, string> Paths
    {
        get
        {
            return new Dictionary<string, string>
            {
                { "mantle-common", "/_content/Mantle.Web.CommonResources/js/mantle-common" },
                { "mantle-knockout-chosen", "/_content/Mantle.Web.CommonResources/js/mantle-knockout-chosen" },
                { "mantle-section-switching", "/_content/Mantle.Web.CommonResources/js/mantle-section-switching" },
                { "mantle-tinymce", "/_content/Mantle.Web.CommonResources/js/tinymce/mantle-tinymce" },
                { "momentjs", "/_content/Mantle.Web.CommonResources/js/moment-with-locales.min" },
                { "grid-helper", "/_content/Mantle.Web.CommonResources/js/grid-helper" },
                { "odata-helpers", "/_content/Mantle.Web.CommonResources/js/odata-helpers" }
            };
        }
    }

    public IDictionary<string, string[]> Shim
    {
        get
        {
            return new Dictionary<string, string[]>
            {
                { "mantle-knockout-chosen", new[] { "chosen", "knockout" } },
                { "mantle-tinymce", new[] { "tinymce" } }
            };
        }
    }

    #endregion IRequireJSConfigProvider Members
}