using System.Collections.Generic;
using Mantle.Web.Infrastructure;

namespace Mantle.Web.CommonResources.Infrastructure
{
    public class RequireJSConfigProvider : IRequireJSConfigProvider
    {
        #region IRequireJSConfigProvider Members

        public IDictionary<string, string> Paths
        {
            get
            {
                var paths = new Dictionary<string, string>();

                paths.Add("mantle-common", "/Mantle.Web.CommonResources.Scripts.mantle-common");
                paths.Add("mantle-chosen-knockout", "/Mantle.Web.CommonResources.Scripts.mantle-knockout-chosen");
                paths.Add("mantle-jqueryval", "/Mantle.Web.CommonResources.Scripts.mantle-jqval");
                paths.Add("mantle-section-switching", "/Mantle.Web.CommonResources.Scripts.mantle-section-switching");
                paths.Add("mantle-tinymce", "/Mantle.Web.CommonResources.Scripts.mantle-tinymce");
                //paths.Add("bootstrap-fileinput", "/Mantle.Web.CommonResources.Scripts.bootstrapFileInput.fileinput.min");
                //paths.Add("bootstrap-slider", "/Mantle.Web.CommonResources.Scripts.bootstrap-slider.min");
                //paths.Add("bootstrap-slider-knockout", "/Mantle.Web.CommonResources.Scripts.bootstrap-slider-knockout-binding");
                //paths.Add("momentjs", "/Mantle.Web.CommonResources.Scripts.moment-with-locales.min");

                return paths;
            }
        }

        public IDictionary<string, string[]> Shim
        {
            get
            {
                var shim = new Dictionary<string, string[]>();

                shim.Add("mantle-chosen-knockout", new[] { "chosen", "knockout" });
                shim.Add("mantle-jqueryval", new[] { "jqueryval" });
                shim.Add("mantle-tinymce", new[] { "tinymce" });
                //shim.Add("bootstrap-fileinput", new[] { "jquery", "bootstrap" });
                //shim.Add("bootstrap-slider", new[] { "jquery", "bootstrap" });
                //shim.Add("bootstrap-slider-knockout", new[] { "jquery", "bootstrap", "knockout" });

                return shim;
            }
        }

        #endregion IRequireJSConfigProvider Members
    }
}