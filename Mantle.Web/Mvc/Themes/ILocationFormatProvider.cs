namespace Mantle.Web.Mvc.Themes
{
    public interface ILocationFormatProvider
    {
        IEnumerable<string> AreaViewLocationFormats { get; }

        IEnumerable<string> AreaMasterLocationFormats { get; }

        IEnumerable<string> AreaPartialViewLocationFormats { get; }

        IEnumerable<string> ViewLocationFormats { get; }

        IEnumerable<string> MasterLocationFormats { get; }

        IEnumerable<string> PartialViewLocationFormats { get; }
    }
}