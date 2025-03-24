namespace Mantle.Plugins.Messaging.Forums.Infrastructure;

public class LocationFormatProvider : ILocationFormatProvider
{
    #region ILocationFormatProvider Members

    public IEnumerable<string> AreaViewLocationFormats => new[]
    {
        "~/Views/Plugins/Messaging.Forums/Areas/{2}/{1}/{0}.cshtml",
        "~/Views/Plugins/Messaging.Forums/Areas/{2}/Shared/{0}.cshtml",
    };

    public IEnumerable<string> AreaMasterLocationFormats => new[]
    {
        "~/Views/Plugins/Messaging.Forums/Areas/{2}/{1}/{0}.cshtml",
        "~/Views/Plugins/Messaging.Forums/Areas/{2}/Shared/{0}.cshtml",
    };

    public IEnumerable<string> AreaPartialViewLocationFormats => new[]
    {
        "~/Views/Plugins/Messaging.Forums/Areas/{2}/{1}/{0}.cshtml",
        "~/Views/Plugins/Messaging.Forums/Areas/{2}/Shared/{0}.cshtml",
    };

    public IEnumerable<string> ViewLocationFormats => new[]
    {
        "~/Views/Plugins/Messaging.Forums/{1}/{0}.cshtml",
        "~/Views/Plugins/Messaging.Forums/Shared/{0}.cshtml",
    };

    public IEnumerable<string> MasterLocationFormats => new[]
    {
        "~/Views/Plugins/Messaging.Forums/{1}/{0}.cshtml",
        "~/Views/Plugins/Messaging.Forums/Shared/{0}.cshtml",
    };

    public IEnumerable<string> PartialViewLocationFormats => new[]
    {
        "~/Views/Plugins/Messaging.Forums/{1}/{0}.cshtml",
        "~/Views/Plugins/Messaging.Forums/Shared/{0}.cshtml",
    };

    #endregion ILocationFormatProvider Members
}