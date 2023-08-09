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
                { "mantle-common", "/Mantle.Web.CommonResources.Scripts.mantle-common" },
                { "mantle-knockout-chosen", "/Mantle.Web.CommonResources.Scripts.mantle-knockout-chosen" },
                { "mantle-section-switching", "/Mantle.Web.CommonResources.Scripts.mantle-section-switching" },
                { "mantle-tinymce", "/Mantle.Web.CommonResources.Scripts.tinymce.mantle-tinymce" },
                { "momentjs", "/Mantle.Web.CommonResources.Scripts.moment-with-locales.min" },
                { "grid-helper", "/Mantle.Web.CommonResources.Scripts.grid-helper" },
                { "odata-helpers", "/Mantle.Web.CommonResources.Scripts.odata-helpers" }
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