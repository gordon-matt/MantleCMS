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
                { "mantle-jqueryval", "/Mantle.Web.CommonResources.Scripts.mantle-jqval" },
                { "mantle-section-switching", "/Mantle.Web.CommonResources.Scripts.mantle-section-switching" },
                { "mantle-tinymce", "/Mantle.Web.CommonResources.Scripts.tinymce.mantle-tinymce" },
                { "bootstrap-fileinput", "/Mantle.Web.CommonResources.Scripts.bootstrapFileInput.fileinput.min" },
                { "bootstrap-slider", "/Mantle.Web.CommonResources.Scripts.bootstrap-slider.min" },
                { "bootstrap-slider-knockout", "/Mantle.Web.CommonResources.Scripts.bootstrap-slider-knockout-binding" },
                { "momentjs", "/Mantle.Web.CommonResources.Scripts.moment-with-locales.min" }
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
                { "mantle-jqueryval", new[] { "jqueryval" } },
                { "mantle-tinymce", new[] { "tinymce" } },
                { "bootstrap-fileinput", new[] { "jquery", "bootstrap" } },
                { "bootstrap-slider", new[] { "jquery", "bootstrap" } },
                { "bootstrap-slider-knockout", new[] { "jquery", "bootstrap", "knockout" } }
            };
        }
    }

    #endregion IRequireJSConfigProvider Members
}