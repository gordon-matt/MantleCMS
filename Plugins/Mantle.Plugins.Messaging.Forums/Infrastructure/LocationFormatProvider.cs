using System.Collections.Generic;
using Mantle.Web.Mvc.Themes;

namespace Mantle.Plugins.Messaging.Forums.Infrastructure
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
                    "~/Views/Plugins/Messaging.Forums/Areas/{2}/{1}/{0}.cshtml",
                    "~/Views/Plugins/Messaging.Forums/Areas/{2}/Shared/{0}.cshtml",
                };
            }
        }

        public IEnumerable<string> AreaMasterLocationFormats
        {
            get
            {
                return new[]
                {
                    "~/Views/Plugins/Messaging.Forums/Areas/{2}/{1}/{0}.cshtml",
                    "~/Views/Plugins/Messaging.Forums/Areas/{2}/Shared/{0}.cshtml",
                };
            }
        }

        public IEnumerable<string> AreaPartialViewLocationFormats
        {
            get
            {
                return new[]
                {
                    "~/Views/Plugins/Messaging.Forums/Areas/{2}/{1}/{0}.cshtml",
                    "~/Views/Plugins/Messaging.Forums/Areas/{2}/Shared/{0}.cshtml",
                };
            }
        }

        public IEnumerable<string> ViewLocationFormats
        {
            get
            {
                return new[]
                {
                    "~/Views/Plugins/Messaging.Forums/{1}/{0}.cshtml",
                    "~/Views/Plugins/Messaging.Forums/Shared/{0}.cshtml",
                };
            }
        }

        public IEnumerable<string> MasterLocationFormats
        {
            get
            {
                return new[]
                {
                    "~/Views/Plugins/Messaging.Forums/{1}/{0}.cshtml",
                    "~/Views/Plugins/Messaging.Forums/Shared/{0}.cshtml",
                };
            }
        }

        public IEnumerable<string> PartialViewLocationFormats
        {
            get
            {
                return new[]
                {
                    "~/Views/Plugins/Messaging.Forums/{1}/{0}.cshtml",
                    "~/Views/Plugins/Messaging.Forums/Shared/{0}.cshtml",
                };
            }
        }

        #endregion ILocationFormatProvider Members
    }
}