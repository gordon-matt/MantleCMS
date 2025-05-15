namespace Mantle.Plugins.Widgets.FullCalendar.Infrastructure;

public class LocationFormatProvider : ILocationFormatProvider
{
    #region ILocationFormatProvider Members

    public IEnumerable<string> AreaViewLocationFormats => new[]
    {
        "~/Views/Plugins/Widgets.FullCalendar/Areas/{2}/{1}/{0}.cshtml",
        "~/Views/Plugins/Widgets.FullCalendar/Areas/{2}/Shared/{0}.cshtml",
    };

    public IEnumerable<string> AreaMasterLocationFormats => new[]
    {
        "~/Views/Plugins/Widgets.FullCalendar/Areas/{2}/{1}/{0}.cshtml",
        "~/Views/Plugins/Widgets.FullCalendar/Areas/{2}/Shared/{0}.cshtml",
    };

    public IEnumerable<string> AreaPartialViewLocationFormats => new[]
    {
        "~/Views/Plugins/Widgets.FullCalendar/Areas/{2}/{1}/{0}.cshtml",
        "~/Views/Plugins/Widgets.FullCalendar/Areas/{2}/Shared/{0}.cshtml",
    };

    public IEnumerable<string> ViewLocationFormats => new[]
    {
        "~/Views/Plugins/Widgets.FullCalendar/{1}/{0}.cshtml",
        "~/Views/Plugins/Widgets.FullCalendar/Shared/{0}.cshtml",
    };

    public IEnumerable<string> MasterLocationFormats => new[]
    {
        "~/Views/Plugins/Widgets.FullCalendar/{1}/{0}.cshtml",
        "~/Views/Plugins/Widgets.FullCalendar/Shared/{0}.cshtml",
    };

    public IEnumerable<string> PartialViewLocationFormats => new[]
    {
        "~/Views/Plugins/Widgets.FullCalendar/{1}/{0}.cshtml",
        "~/Views/Plugins/Widgets.FullCalendar/Shared/{0}.cshtml",
    };

    #endregion ILocationFormatProvider Members
}