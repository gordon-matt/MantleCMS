using System;
using System.Linq;
using System.Xml.Linq;

namespace Mantle.Web.Indexing.Configuration
{
    public class IndexSettings
    {
        public DateTime LastIndexedUtc { get; set; }

        public static readonly string TagSettings = "Settings";
        public static readonly string TagMode = "Mode";
        public static readonly string TagLastIndexedUtc = "LastIndexedUtc";

        public IndexSettings()
        {
            LastIndexedUtc = DateTime.MinValue;
        }

        public static IndexSettings Parse(string content)
        {
            try
            {
                var doc = XDocument.Parse(content);

                return new IndexSettings
                {
                    LastIndexedUtc = DateTime.Parse(doc.Descendants(TagLastIndexedUtc).First().Value).ToUniversalTime()
                };
            }
            catch
            {
                return new IndexSettings();
            }
        }

        public string ToXml()
        {
            return new XDocument(
                    new XElement(TagSettings,
                        new XElement(TagLastIndexedUtc, LastIndexedUtc.ToString("u"))
            )).ToString();
        }
    }
}