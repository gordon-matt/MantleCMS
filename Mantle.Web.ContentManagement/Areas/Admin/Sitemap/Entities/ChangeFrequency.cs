using System.Xml.Serialization;

namespace Mantle.Web.ContentManagement.Areas.Admin.Sitemap.Entities;

public enum ChangeFrequency : byte
{
    [XmlEnum("always")]
    Always = 0,

    [XmlEnum("hourly")]
    Hourly = 1,

    [XmlEnum("daily")]
    Daily = 2,

    [XmlEnum("weekly")]
    Weekly = 3,

    [XmlEnum("monthly")]
    Monthly = 4,

    [XmlEnum("yearly")]
    Yearly = 5,

    [XmlEnum("never")]
    Never = 6
}