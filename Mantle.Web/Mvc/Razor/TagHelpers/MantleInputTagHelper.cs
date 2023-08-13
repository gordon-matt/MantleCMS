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
    protected const string HELP_ATTRIBUTE_NAME = "asp-help";
    protected const string ICON_ATTRIBUTE_NAME = "asp-icon";
    protected const string LABEL_ATTRIBUTE_NAME = "asp-label";
    protected const string VALIDATION_MSG_ATTRIBUTE_NAME = "asp-validation-msg";

    private readonly IHtmlHelper htmlHelper;

    public MantleInputTagHelper(IHtmlGenerator generator, IHtmlHelper htmlHelper)
        : base(generator)
    {
        this.htmlHelper = htmlHelper;
    }

    [HtmlAttributeName(BIND_ATTRIBUTE_NAME)]
    public string KnockoutBinding { set; get; }

    [HtmlAttributeName(HELP_ATTRIBUTE_NAME)]
    public string HelpText { set; get; }

    [HtmlAttributeName(ICON_ATTRIBUTE_NAME)]
    public string Icon { set; get; }

    [HtmlAttributeName(LABEL_ATTRIBUTE_NAME)]
    public string Label { set; get; }

    [HtmlAttributeName(VALIDATION_MSG_ATTRIBUTE_NAME)]
    public bool ValidationMessage { set; get; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        var viewContextAware = htmlHelper as IViewContextAware;
        viewContextAware?.Contextualize(ViewContext);

        string preContent = string.Empty;
        string postContent = string.Empty;

        output.TagName = "input";

        string koBinding = KnockoutBinding;
        if (koBinding is null)
        {
            if (For.Name.Contains('.'))
            {
                string[] parts = For.Name.Split('.');
                koBinding = string.Join(".", parts.Select(x => x.Camelize()));
            }
            else
            {
                koBinding = For.Name.Camelize();
            }
        }

        if (For.ModelExplorer.ModelType == typeof(bool))
        {
            output.Attributes.Add("data-bind", $"checked: {koBinding}");
            preContent = @"<div class=""checkbox""><label>";
            postContent = $@"&nbsp;{Label}</label></div>";
        }
        else
        {
            output.AddClass("form-control", HtmlEncoder.Default);
            output.Attributes.Add("data-bind", $"value: {koBinding}");

            //string labelText;
            //if (!string.IsNullOrEmpty(Label))
            //{
            //    labelText = Label;
            //}
            //else
            //{
            //    var modelType = For.ModelExplorer.ModelType;
            //    var property = modelType.GetProperty(For.Name);
            //    if (property != null)
            //    {
            //        var attribute = property.GetCustomAttribute<LocalizedDisplayName>(); // Can't get.. it's in CMS project..
            //    }
            //}

            preContent = $@"<div class=""mb-3"">{htmlHelper.Label(For.Name, Label, new { @class = "form-label" }).GetString()}";

            if (ValidationMessage)
            {
                postContent += htmlHelper.ValidationMessage(For.Name).GetString();
            }

            if (!string.IsNullOrWhiteSpace(HelpText))
            {
                postContent += htmlHelper.HelpText(HelpText).GetString();
            }

            postContent += "</div>";

            if (!string.IsNullOrWhiteSpace(Icon))
            {
                preContent += $@"<div class=""input-group""><span class=""input-group-text""><i class=""{Icon}""></i></span>";
                postContent += "</div>";
            }
        }

        output.PreElement.SetHtmlContent(preContent);
        output.PostElement.SetHtmlContent(postContent);

        base.Process(context, output);
    }
}