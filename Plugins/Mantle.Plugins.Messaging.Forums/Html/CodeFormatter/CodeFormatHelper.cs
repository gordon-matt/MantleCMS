// Taken from nopCommerce v3.7

using System.Text.RegularExpressions;

namespace Mantle.Plugins.Messaging.Forums.Html.CodeFormatter;

/// <summary>
/// Represents a code format helper
/// </summary>
public partial class CodeFormatHelper
{
    #region Fields

    //private static Regex regexCode1 = new Regex(@"(?<begin>\[code:(?<lang>.*?)(?:;ln=(?<linenumbers>(?:on|off)))?(?:;alt=(?<altlinenumbers>(?:on|off)))?(?:;(?<title>.*?))?\])(?<code>.*?)(?<end>\[/code\])", RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase | RegexOptions.Singleline);
    private static readonly Regex regexHtml = new("<[^>]*>", RegexOptions.Compiled);

    private static readonly Regex regexCode2 = new(@"\[code\](?<inner>(.*?))\[/code\]", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    #endregion Fields

    #region Utilities

    /// <summary>
    /// Code evaluator method
    /// </summary>
    /// <param name="match">Match</param>
    /// <returns>Formatted text</returns>
    private static string CodeEvaluator(Match match)
    {
        if (!match.Success)
        {
            return match.Value;
        }

        var options = new HighlightOptions
        {
            Language = match.Groups["lang"].Value,
            Code = match.Groups["code"].Value,
            DisplayLineNumbers = match.Groups["linenumbers"].Value == "on",
            Title = match.Groups["title"].Value,
            AlternateLineNumbers = match.Groups["altlinenumbers"].Value == "on"
        };

        string result = match.Value.Replace(match.Groups["begin"].Value, "");
        result = result.Replace(match.Groups["end"].Value, "");
        result = Highlight(options, result);
        return result;
    }

    /// <summary>
    /// Code evaluator method
    /// </summary>
    /// <param name="match">Match</param>
    /// <returns>Formatted text</returns>
    private static string CodeEvaluatorSimple(Match match)
    {
        if (!match.Success)
        {
            return match.Value;
        }

        var options = new HighlightOptions
        {
            Language = "c#",
            Code = match.Groups["inner"].Value,
            DisplayLineNumbers = false,
            Title = string.Empty,
            AlternateLineNumbers = false
        };

        string result = match.Value;
        result = Highlight(options, result);
        return result;
    }

    /// <summary>
    /// Strips HTML
    /// </summary>
    /// <param name="html">HTML</param>
    /// <returns>Formatted text</returns>
    private static string StripHtml(string html) => string.IsNullOrEmpty(html) ? string.Empty : regexHtml.Replace(html, string.Empty);

    /// <summary>
    /// Returns the formatted text.
    /// </summary>
    /// <param name="options">Whatever options were set in the regex groups.</param>
    /// <param name="text">Send the e.body so it can get formatted.</param>
    /// <returns>The formatted string of the match.</returns>
    private static string Highlight(HighlightOptions options, string text)
    {
        switch (options.Language)
        {
            case "c#":
                var csf = new CSharpFormat
                {
                    LineNumbers = options.DisplayLineNumbers,
                    Alternate = options.AlternateLineNumbers
                };
                return csf.FormatCode(text).HtmlDecode();

            case "vb":
                var vbf = new VisualBasicFormat
                {
                    LineNumbers = options.DisplayLineNumbers,
                    Alternate = options.AlternateLineNumbers
                };
                return vbf.FormatCode(text);

            case "js":
                var jsf = new JavaScriptFormat
                {
                    LineNumbers = options.DisplayLineNumbers,
                    Alternate = options.AlternateLineNumbers
                };
                return jsf.FormatCode(text).HtmlDecode();

            case "html":
                var htmlf = new HtmlFormat
                {
                    LineNumbers = options.DisplayLineNumbers,
                    Alternate = options.AlternateLineNumbers
                };
                text = StripHtml(text).Trim();
                string code = htmlf.FormatCode(text.HtmlDecode()).Trim();
                return code.Replace("\r\n", "<br />").Replace("\n", "<br />");

            case "xml":
                var xmlf = new HtmlFormat
                {
                    LineNumbers = options.DisplayLineNumbers,
                    Alternate = options.AlternateLineNumbers
                };
                text = text.Replace("<br />", "\r\n");
                text = StripHtml(text).Trim();
                string xml = xmlf.FormatCode(text.HtmlDecode()).Trim();
                return xml.Replace("\r\n", "<br />").Replace("\n", "<br />");

            case "tsql":
                var tsqlf = new TsqlFormat
                {
                    LineNumbers = options.DisplayLineNumbers,
                    Alternate = options.AlternateLineNumbers
                };
                return tsqlf.FormatCode(text).HtmlDecode();

            case "msh":
                var mshf = new MshFormat
                {
                    LineNumbers = options.DisplayLineNumbers,
                    Alternate = options.AlternateLineNumbers
                };
                return mshf.FormatCode(text).HtmlDecode();
        }

        return string.Empty;
    }

    #endregion Utilities

    #region Methods

    /// <summary>
    /// Formats the text
    /// </summary>
    /// <param name="text">Text</param>
    /// <returns>Formatted text</returns>
    public static string FormatTextSimple(string text)
    {
        if (string.IsNullOrEmpty(text))
        {
            return string.Empty;
        }

        if (text.Contains("[/code]"))
        {
            text = regexCode2.Replace(text, new MatchEvaluator(CodeEvaluatorSimple));
            text = regexCode2.Replace(text, "$1");
        }
        return text;
    }

    #endregion Methods
}