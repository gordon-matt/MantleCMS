namespace Mantle.Web.ContentManagement.Infrastructure;

public class LocationFormatProvider : ILocationFormatProvider
{
    #region ILocationFormatProvider Members

    public IEnumerable<string> AreaViewLocationFormats => new[]
    {
        "~/Views/Mantle.Web.ContentManagement/Areas/{2}/{1}/{0}.cshtml",
        "~/Views/Mantle.Web.ContentManagement/Areas/{2}/Shared/{0}.cshtml",
    };

    public IEnumerable<string> AreaMasterLocationFormats => new[]
    {
        "~/Views/Mantle.Web.ContentManagement/Areas/{2}/{1}/{0}.cshtml",
        "~/Views/Mantle.Web.ContentManagement/Areas/{2}/Shared/{0}.cshtml",
    };

    public IEnumerable<string> AreaPartialViewLocationFormats => new[]
    {
        "~/Views/Mantle.Web.ContentManagement/Areas/{2}/{1}/{0}.cshtml",
        "~/Views/Mantle.Web.ContentManagement/Areas/{2}/Shared/{0}.cshtml",
    };

    public IEnumerable<string> ViewLocationFormats => new[]
    {
        "~/Views/Mantle.Web.ContentManagement/{1}/{0}.cshtml",
        "~/Views/Mantle.Web.ContentManagement/Shared/{0}.cshtml",
    };

    public IEnumerable<string> MasterLocationFormats => new[]
    {
        "~/Views/Mantle.Web.ContentManagement/{1}/{0}.cshtml",
        "~/Views/Mantle.Web.ContentManagement/Shared/{0}.cshtml",
    };

    public IEnumerable<string> PartialViewLocationFormats => new[]
    {
        "~/Views/Mantle.Web.ContentManagement/{1}/{0}.cshtml",
        "~/Views/Mantle.Web.ContentManagement/Shared/{0}.cshtml",
    };

    #endregion ILocationFormatProvider Members
}