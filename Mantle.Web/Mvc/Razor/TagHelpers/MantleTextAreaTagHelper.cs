using Extenso.AspNetCore.Mvc.Html;
using Humanizer;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Mantle.Web.Mvc.Razor.TagHelpers;

[HtmlTargetElement("mantle-textarea", Attributes = FOR_ATTRIBUTE_NAME)]
public class MantleTextAreaTagHelper : TextAreaTagHelper
{
    protected const string BIND_ATTRIBUTE_NAME = "ko-bind";
    protected const string IS_RTE = "asp-richtext";
    protected const string BIND_RTE_CONFIG = "ko-richtext-config";
    protected const string ROWS_ATTRIBUTE_NAME = "asp-rows";
    protected const string FOR_ATTRIBUTE_NAME = "asp-for";
    protected const string HELP_ATTRIBUTE_NAME = "asp-help";
    protected const string LABEL_ATTRIBUTE_NAME = "asp-label";
    protected const string VALIDATION_MSG_ATTRIBUTE_NAME = "asp-validation-msg";

    private readonly IHtmlHelper htmlHelper;

    public MantleTextAreaTagHelper(IHtmlGenerator generator, IHtmlHelper htmlHelper)
        : base(generator)
    {
        this.htmlHelper = htmlHelper;
    }

    [HtmlAttributeName(BIND_ATTRIBUTE_NAME)]
    public string Bind { set; get; }

    [HtmlAttributeName(IS_RTE)]
    public bool IsRichTextEditor { set; get; }

    [HtmlAttributeName(BIND_RTE_CONFIG)]
    public string BindRichTextConfig { set; get; }

    [HtmlAttributeName(ROWS_ATTRIBUTE_NAME)]
    public int Rows { set; get; }

    [HtmlAttributeName(HELP_ATTRIBUTE_NAME)]
    public string HelpText { set; get; }

    [HtmlAttributeName(LABEL_ATTRIBUTE_NAME)]
    public string Label { set; get; }

    [HtmlAttributeName(VALIDATION_MSG_ATTRIBUTE_NAME)]
    public bool ValidationMessage { set; get; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        if (output == null)
            throw new ArgumentNullException(nameof(output));

        var viewContextAware = htmlHelper as IViewContextAware;
        viewContextAware?.Contextualize(ViewContext);

        string preContent = string.Empty;
        string postContent = string.Empty;

        //tag details
        output.TagName = "textarea";
        output.TagMode = TagMode.StartTagAndEndTag;

        //merge classes
        string @class = output.Attributes.ContainsName("class")
            ? $"{output.Attributes["class"].Value} form-control"
            : "form-control";
        output.Attributes.SetAttribute("class", @class);

        if (IsRichTextEditor)
        {
            output.Attributes.Add("data-bind", $"wysiwyg: {Bind ?? For.Name.Camelize()}, wysiwygConfig: {BindRichTextConfig}");
        }
        else
        {
            output.Attributes.Add("data-bind", $"value: {Bind ?? For.Name.Camelize()}");
        }

        preContent = $@"<div class=""mb-3"">{htmlHelper.Label(For.Name, Label, new { @class = "form-label" }).GetString()}";

        if (ValidationMessage)
        {
            postContent += htmlHelper.ValidationMessage(For.Name).GetString();
        }

        if (!string.IsNullOrWhiteSpace(HelpText))
        {
            postContent += htmlHelper.HelpText(For.Name).GetString();
        }

        postContent += "</div>";

        //additional parameters
        output.Attributes.SetAttribute("rows", Rows > 0 ? Rows : 4);

        output.PreElement.SetHtmlContent(preContent);
        output.PostElement.SetHtmlContent(postContent);

        base.Process(context, output);
    }
}