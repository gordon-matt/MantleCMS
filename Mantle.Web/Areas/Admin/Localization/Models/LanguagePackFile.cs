namespace Mantle.Web.Areas.Admin.Localization.Models;

public class LanguagePackFile
{
    public string CultureCode { get; set; }

    public IDictionary<string, string> LocalizedStrings { get; set; }
}