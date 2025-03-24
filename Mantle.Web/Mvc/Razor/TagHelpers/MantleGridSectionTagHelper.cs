using System.Globalization;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Mantle.Web.Mvc.Razor.TagHelpers;

[HtmlTargetElement("mantle-grid", TagStructure = TagStructure.WithoutEndTag)]
public class MantleGridSectionTagHelper : TagHelper
{
    protected const string ID_ATTRIBUTE_NAME = "id";

    [HtmlAttributeName(ID_ATTRIBUTE_NAME)]
    public string Id { set; get; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(output);

        //clear the output
        output.SuppressOutput();

        output.Content.AppendHtml(
$@" <div class=""clearfix""></div>
    <br />
    <div class=""col-xs-12 col-sm-12 col-md-12 col-lg-12{(CultureInfo.CurrentCulture.TextInfo.IsRightToLeft ? " k-rtl" : string.Empty)}"">
        <div id=""{Id ?? "Grid"}""></div>
    </div>");
    }
}