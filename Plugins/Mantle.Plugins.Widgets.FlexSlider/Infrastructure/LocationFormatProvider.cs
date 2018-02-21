using System.Collections.Generic;
using Mantle.Web.Mvc.Themes;

namespace Mantle.Plugins.Widgets.FlexSlider.Infrastructure
{
    public class LocationFormatProvider : ILocationFormatProvider
    {
        #region ILocationFormatProvider Members

        public IEnumerable<string> AreaViewLocationFormats
        {
            get
            {
                return new[]
                {
                    //themes
                    "~/Areas/{2}/Themes/{3}/Views/Plugins/Widgets.FlexSlider/{1}/{0}.cshtml",
                    "~/Areas/{2}/Themes/{3}/Views/Plugins/Widgets.FlexSlider/Shared/{0}.cshtml",

                    //shared
                    "~/Areas/{2}/Views/Plugins/Widgets.FlexSlider/{1}/{0}.cshtml",
                    "~/Areas/{2}/Views/Plugins/Widgets.FlexSlider/Shared/{0}.cshtml",

                    //default
                    "~/Plugins/Widgets.FlexSlider/Areas/{2}/Views/{1}/{0}.cshtml",
                    "~/Plugins/Widgets.FlexSlider/Areas/{2}/Views/Shared/{0}.cshtml",
                };
            }
        }

        public IEnumerable<string> AreaMasterLocationFormats
        {
            get
            {
                return new[]
                {
                    //themes
                    "~/Areas/{2}/Themes/{3}/Views/Plugins/Widgets.FlexSlider/{1}/{0}.cshtml",
                    "~/Areas/{2}/Themes/{3}/Views/Plugins/Widgets.FlexSlider/Shared/{0}.cshtml",

                    //shared
                    "~/Areas/{2}/Views/Plugins/Widgets.FlexSlider/{1}/{0}.cshtml",
                    "~/Areas/{2}/Views/Plugins/Widgets.FlexSlider/Shared/{0}.cshtml",

                    //default
                    "~/Plugins/Widgets.FlexSlider/Areas/{2}/Views/{1}/{0}.cshtml",
                    "~/Plugins/Widgets.FlexSlider/Areas/{2}/Views/Shared/{0}.cshtml",
                };
            }
        }

        public IEnumerable<string> AreaPartialViewLocationFormats
        {
            get
            {
                return new[]
                {
                    //themes
                    "~/Areas/{2}/Themes/{3}/Views/Plugins/Widgets.FlexSlider/{1}/{0}.cshtml",
                    "~/Areas/{2}/Themes/{3}/Views/Plugins/Widgets.FlexSlider/Shared/{0}.cshtml",

                    //shared
                    "~/Areas/{2}/Views/Plugins/Widgets.FlexSlider/{1}/{0}.cshtml",
                    "~/Areas/{2}/Views/Plugins/Widgets.FlexSlider/Shared/{0}.cshtml",

                    //default
                    "~/Plugins/Widgets.FlexSlider/Areas/{2}/Views/{1}/{0}.cshtml",
                    "~/Plugins/Widgets.FlexSlider/Areas/{2}/Views/Shared/{0}.cshtml",
                };
            }
        }

        public IEnumerable<string> ViewLocationFormats
        {
            get
            {
                return new[]
                {
                    //themes
                    "~/Themes/{2}/Views/Plugins/Widgets.FlexSlider/{1}/{0}.cshtml",
                    "~/Themes/{2}/Views/Plugins/Widgets.FlexSlider/Shared/{0}.cshtml",

                    //shared
                    "~/Views/Plugins/Widgets.FlexSlider/{1}/{0}.cshtml",
                    "~/Views/Plugins/Widgets.FlexSlider/Shared/{0}.cshtml",

                    //default
                    "~/Plugins/Widgets.FlexSlider/Views/{1}/{0}.cshtml",
                    "~/Plugins/Widgets.FlexSlider/Views/Shared/{0}.cshtml",
                };
            }
        }

        public IEnumerable<string> MasterLocationFormats
        {
            get
            {
                return new[]
                {
                    //themes
                    "~/Themes/{2}/Views/Plugins/Widgets.FlexSlider/{1}/{0}.cshtml",
                    "~/Themes/{2}/Views/Plugins/Widgets.FlexSlider/Shared/{0}.cshtml",

                    //shared
                    "~/Views/Plugins/Widgets.FlexSlider/{1}/{0}.cshtml",
                    "~/Views/Plugins/Widgets.FlexSlider/Shared/{0}.cshtml",

                    //default
                    "~/Plugins/Widgets.FlexSlider/Views/{1}/{0}.cshtml",
                    "~/Plugins/Widgets.FlexSlider/Views/Shared/{0}.cshtml",
                };
            }
        }

        public IEnumerable<string> PartialViewLocationFormats
        {
            get
            {
                return new[]
                {
                    //themes
                    "~/Themes/{2}/Views/Plugins/Widgets.FlexSlider/{1}/{0}.cshtml",
                    "~/Themes/{2}/Views/Plugins/Widgets.FlexSlider/Shared/{0}.cshtml",

                    //shared
                    "~/Views/Plugins/Widgets.FlexSlider/{1}/{0}.cshtml",
                    "~/Views/Plugins/Widgets.FlexSlider/Shared/{0}.cshtml",

                    //default
                    "~/Plugins/Widgets.FlexSlider/Views/{1}/{0}.cshtml",
                    "~/Plugins/Widgets.FlexSlider/Views/Shared/{0}.cshtml",
                };
            }
        }

        #endregion ILocationFormatProvider Members
    }
}