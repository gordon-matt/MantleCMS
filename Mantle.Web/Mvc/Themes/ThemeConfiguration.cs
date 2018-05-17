using System.IO;
using System.Xml;
using System.Xml.Serialization;
using Extenso.IO;

namespace Mantle.Web.Mvc.Themes
{
    [XmlRoot("Theme")]
    public class ThemeConfiguration
    {
        public static ThemeConfiguration Load(string path, string themeName)
        {
            var themeConfig = new FileInfo(path).XmlDeserialize<ThemeConfiguration>();
            themeConfig.ThemeName = themeName;
            return themeConfig;
        }

        [XmlIgnore]
        public string ThemeName { get; set; }

        [XmlAttribute("title")]
        public string ThemeTitle { get; set; }

        [XmlAttribute("supportRTL")]
        public bool SupportRtl { get; set; }

        [XmlAttribute("previewImageUrl")]
        public string PreviewImageUrl { get; set; }

        [XmlAttribute("previewText")]
        public string PreviewText { get; set; }

        [XmlAttribute("defaultLayoutPath")]
        public string DefaultLayoutPath { get; set; }
    }
}