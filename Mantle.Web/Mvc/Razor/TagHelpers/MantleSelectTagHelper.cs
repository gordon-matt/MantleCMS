using Extenso.AspNetCore.Mvc.Html;
using Humanizer;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;

namespace Mantle.Web.Mvc.Razor.TagHelpers;

[HtmlTargetElement("mantle-select", TagStructure = TagStructure.WithoutEndTag)]
public class MantleSelectTagHelper : TagHelper
{
    protected const string BIND_ATTRIBUTE_NAME = "ko-bind";
    protected const string FOR_ATTRIBUTE_NAME = "asp-for";
    protected const string HELP_ATTRIBUTE_NAME = "asp-help";
    protected const string ICON_ATTRIBUTE_NAME = "asp-icon";
    protected const string ITEMS_ATTRIBUTE_NAME = "asp-items";
    protected const string LABEL_ATTRIBUTE_NAME = "asp-label";
    protected const string NAME_ATTRIBUTE_NAME = "asp-for-name";
    protected const string REQUIRED_ATTRIBUTE_NAME = "asp-required";
    protected const string VALIDATION_MSG_ATTRIBUTE_NAME = "asp-validation-msg";

    private readonly IHtmlHelper htmlHelper;

    public MantleSelectTagHelper(IHtmlHelper htmlHelper)
    {
        this.htmlHelper = htmlHelper;
    }

    /// <summary>
    /// An expression to be evaluated against the current model
    /// </summary>
    [HtmlAttributeName(FOR_ATTRIBUTE_NAME)]
    public ModelExpression For { get; set; }

    /// <summary>
    /// Name for a dropdown list
    /// </summary>
    [HtmlAttributeName(NAME_ATTRIBUTE_NAME)]
    public string Name { get; set; }

    /// <summary>
    /// Items for a dropdown list
    /// </summary>
    [HtmlAttributeName(ITEMS_ATTRIBUTE_NAME)]
    public IEnumerable<SelectListItem> Items { set; get; } = Enumerable.Empty<SelectListItem>();

    [HtmlAttributeName(BIND_ATTRIBUTE_NAME)]
    public string Bind { set; get; }

    [HtmlAttributeName(HELP_ATTRIBUTE_NAME)]
    public string HelpText { set; get; }

    [HtmlAttributeName(ICON_ATTRIBUTE_NAME)]
    public string Icon { set; get; }

    [HtmlAttributeName(LABEL_ATTRIBUTE_NAME)]
    public string Label { set; get; }

    [HtmlAttributeName(VALIDATION_MSG_ATTRIBUTE_NAME)]
    public bool ValidationMessage { set; get; }

    /// <summary>
    /// ViewContext
    /// </summary>
    [HtmlAttributeNotBound]
    [ViewContext]
    public ViewContext ViewContext { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (context == null)
            throw new ArgumentNullException(nameof(context));

        if (output == null)
            throw new ArgumentNullException(nameof(output));

        //clear the output
        output.SuppressOutput();

        string tagName = For != null ? For.Name : Name;

        var viewContextAware = htmlHelper as IViewContextAware;
        viewContextAware?.Contextualize(ViewContext);

        string preContent = string.Empty;
        string postContent = string.Empty;

        preContent = $@"<div class=""mb-3"">{htmlHelper.Label(tagName, Label, new { @class = "form-label" }).GetString()}";

        if (ValidationMessage)
        {
            postContent += htmlHelper.ValidationMessage(tagName).GetString();
        }

        if (!string.IsNullOrWhiteSpace(HelpText))
        {
            postContent += htmlHelper.HelpText(tagName).GetString();
        }

        postContent += "</div>";

        if (!string.IsNullOrWhiteSpace(Icon))
        {
            preContent += $@"<div class=""input-group""><span class=""input-group-text""><i class=""{Icon}""></i></span>";
            postContent += "</div>";
        }

        output.PreElement.SetHtmlContent(preContent);
        output.PostElement.SetHtmlContent(postContent);

        // Get htmlAttributes object
        var htmlAttributes = new Dictionary<string, object>();
        var attributes = context.AllAttributes;
        foreach (var attribute in attributes)
        {
            if (!attribute.Name.In(FOR_ATTRIBUTE_NAME, NAME_ATTRIBUTE_NAME, ITEMS_ATTRIBUTE_NAME, REQUIRED_ATTRIBUTE_NAME, BIND_ATTRIBUTE_NAME))
            {
                htmlAttributes.Add(attribute.Name, attribute.Value);
            }
        }

        htmlAttributes.Add("data-bind", $"value: {Bind ?? For.Name.Camelize()}");

        // Generate editor
        if (!string.IsNullOrEmpty(tagName))
        {
            if (htmlAttributes.ContainsKey("class"))
            {
                htmlAttributes["class"] += " form-control";
            }
            else
            {
                htmlAttributes.Add("class", "form-control");
            }

            var dropDownList = htmlHelper.DropDownList(tagName, Items, htmlAttributes);
            output.Content.SetHtmlContent(dropDownList.GetString());
        }
    }
}