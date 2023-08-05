using Extenso.AspNetCore.Mvc.ExtensoUI;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Mantle.Web.Mvc.Razor.TagHelpers;

[HtmlTargetElement("mantle-ko-button", TagStructure = TagStructure.WithoutEndTag)]
public class MantleKOButtonTagHelper : TagHelper
{
    protected const string CLICK_ATTRIBUTE_NAME = "ko-click";
    protected const string ICON_ATTRIBUTE_NAME = "icon";
    protected const string TEXT_ATTRIBUTE_NAME = "text";
    protected const string STATE_ATTRIBUTE_NAME = "state";

    [HtmlAttributeName(CLICK_ATTRIBUTE_NAME)]
    public string Click { set; get; }

    [HtmlAttributeName(ICON_ATTRIBUTE_NAME)]
    public string Icon { set; get; }

    [HtmlAttributeName(TEXT_ATTRIBUTE_NAME)]
    public string Text { set; get; }

    [HtmlAttributeName(STATE_ATTRIBUTE_NAME)]
    public State State { set; get; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        if (output == null)
            throw new ArgumentNullException(nameof(output));

        //clear the output
        output.SuppressOutput();

        var tagBuilder = new FluentTagBuilder("button")
            .AddCssClass(GetButtonCssClass(State))
            .MergeAttribute("data-bind", $"click: {Click}");

        if (!string.IsNullOrWhiteSpace(Icon))
        {
            tagBuilder = tagBuilder.StartTag("i").AddCssClass(Icon).EndTag().AppendContent("&nbsp;");
        }

        tagBuilder.AppendContent(Text);

        output.Content.AppendHtml(tagBuilder.ToString());
    }

    protected virtual string GetButtonCssClass(State state)
    {
        return state switch
        {
            State.Danger => "btn btn-danger",
            State.Default => "btn btn-default",
            State.Info => "btn btn-info",
            State.Inverse => "btn btn-inverse",
            State.Primary => "btn btn-primary",
            State.Success => "btn btn-success",
            State.Warning => "btn btn-warning",
            _ => "btn btn-default",
        };
    }
}