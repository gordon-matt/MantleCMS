using Extenso.AspNetCore.Mvc.Html;
using Humanizer;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Text.Encodings.Web;

namespace Mantle.Web.Mvc.Razor.TagHelpers;

[HtmlTargetElement("mantle-input", Attributes = FOR_ATTRIBUTE_NAME)]
public class MantleInputTagHelper : InputTagHelper
{
    protected const string BIND_ATTRIBUTE_NAME = "ko-bind";
    protected const string FOR_ATTRIBUTE_NAME = "asp-for";
    protected const string ICON_ATTRIBUTE_NAME = "asp-icon";
    protected const string LABEL_ATTRIBUTE_NAME = "asp-label";
    private readonly IHtmlHelper htmlHelper;

    public MantleInputTagHelper(IHtmlGenerator generator, IHtmlHelper htmlHelper)
        : base(generator)
    {
        this.htmlHelper = htmlHelper;
    }

    [HtmlAttributeName(BIND_ATTRIBUTE_NAME)]
    public string Bind { set; get; }

    [HtmlAttributeName(ICON_ATTRIBUTE_NAME)]
    public string Icon { set; get; }

    [HtmlAttributeName(LABEL_ATTRIBUTE_NAME)]
    public string Label { set; get; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var viewContextAware = htmlHelper as IViewContextAware;
        viewContextAware?.Contextualize(ViewContext);

        string preContent;
        string postContent;

        output.TagName = "input";
        if (For.ModelExplorer.ModelType == typeof(bool))
        {
            output.Attributes.Add("data-bind", $"checked: {Bind ?? For.Name.Camelize()}");
            preContent = @"<div class=""checkbox""><label>";
            postContent = $@"&nbsp;{Label}</label></div>";
        }
        else
        {
            output.AddClass("form-control", HtmlEncoder.Default);
            output.Attributes.Add("data-bind", $"value: {Bind ?? For.Name.Camelize()}");
            preContent = $@"<div class=""form-group"">{htmlHelper.Label(For.Name, Label, new { @class = "control-label" }).GetString()}";
            postContent = "</div>";

            if (!string.IsNullOrWhiteSpace(Icon))
            {
                preContent += $@"<div class=""input-group""><span class=""input-group-addon""><i class=""{Icon}""></i></span>";
                postContent += "</div>";
            }
        }

        output.PreElement.SetHtmlContent(preContent);
        output.PostElement.SetHtmlContent(postContent);

        base.Process(context, output);
    }
}