using System.Collections.Generic;
using Mantle.Web.Mvc.Themes;

namespace Mantle.Web.ContentManagement.Infrastructure
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
                    "~/Views/Mantle.Web.ContentManagement/Areas/{2}/{1}/{0}.cshtml",
                    "~/Views/Mantle.Web.ContentManagement/Areas/{2}/Shared/{0}.cshtml",
                };
            }
        }

        public IEnumerable<string> AreaMasterLocationFormats
        {
            get
            {
                return new[]
                {
                    "~/Views/Mantle.Web.ContentManagement/Areas/{2}/{1}/{0}.cshtml",
                    "~/Views/Mantle.Web.ContentManagement/Areas/{2}/Shared/{0}.cshtml",
                };
            }
        }

        public IEnumerable<string> AreaPartialViewLocationFormats
        {
            get
            {
                return new[]
                {
                    "~/Views/Mantle.Web.ContentManagement/Areas/{2}/{1}/{0}.cshtml",
                    "~/Views/Mantle.Web.ContentManagement/Areas/{2}/Shared/{0}.cshtml",
                };
            }
        }

        public IEnumerable<string> ViewLocationFormats
        {
            get
            {
                return new[]
                {
                    "~/Views/Mantle.Web.ContentManagement/{1}/{0}.cshtml",
                    "~/Views/Mantle.Web.ContentManagement/Shared/{0}.cshtml",
                };
            }
        }

        public IEnumerable<string> MasterLocationFormats
        {
            get
            {
                return new[]
                {
                    "~/Views/Mantle.Web.ContentManagement/{1}/{0}.cshtml",
                    "~/Views/Mantle.Web.ContentManagement/Shared/{0}.cshtml",
                };
            }
        }

        public IEnumerable<string> PartialViewLocationFormats
        {
            get
            {
                return new[]
                {
                    "~/Views/Mantle.Web.ContentManagement/{1}/{0}.cshtml",
                    "~/Views/Mantle.Web.ContentManagement/Shared/{0}.cshtml",
                };
            }
        }

        #endregion ILocationFormatProvider Members
    }
}