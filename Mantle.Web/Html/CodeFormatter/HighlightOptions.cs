// Taken from nopCommerce v3.7

namespace Mantle.Web.Html.CodeFormatter;

/// <summary>
/// Handles all of the options for changing the rendered code.
/// </summary>
public partial class HighlightOptions
{
    public string Code { get; set; }

    public bool DisplayLineNumbers { get; set; }

    public string Language { get; set; }

    public string Title { get; set; }

    public bool AlternateLineNumbers { get; set; }
}