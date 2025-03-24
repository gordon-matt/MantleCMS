using System.Text.Encodings.Web;
using Mantle.Web.Mvc.Resources;
using Microsoft.AspNetCore.Mvc.Razor.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Mantle.Web.Mvc.Razor.TagHelpers;

/// <summary>
/// CSS bundling tag helper
/// </summary>
[HtmlTargetElement("mantle-link", Attributes = "[rel=stylesheet]")]
public partial class MantleLinkTagHelper : UrlResolutionTagHelper
{
    #region Constants

    protected const string EXCLUDE_FROM_BUNDLE_ATTRIBUTE_NAME = "asp-exclude-from-bundle";
    protected const string HREF_ATTRIBUTE_NAME = "href";

    #endregion Constants

    #region Fields

    protected readonly IMantleHtmlHelper mantleHtmlHelper;

    #endregion Fields

    #region Ctor

    public MantleLinkTagHelper(
        HtmlEncoder htmlEncoder,
        IMantleHtmlHelper mantleHtmlHelper,
        IUrlHelperFactory urlHelperFactory)
        : base(urlHelperFactory, htmlEncoder)
    {
        this.mantleHtmlHelper = mantleHtmlHelper;
    }

    #endregion Ctor

    #region Methods

    public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
    {
        if (context is null)
        {
            throw new ArgumentNullException(nameof(context));
        }

        if (output == null)
        {
            throw new ArgumentNullException(nameof(output));
        }

        mantleHtmlHelper.AddCssFileParts(Href, string.Empty, ExcludeFromBundle);

        output.SuppressOutput();

        return Task.CompletedTask;
    }

    #endregion Methods

    #region Properties

    /// <summary>
    /// A value indicating if a file should be excluded from the bundle
    /// </summary>
    [HtmlAttributeName(EXCLUDE_FROM_BUNDLE_ATTRIBUTE_NAME)]
    public bool ExcludeFromBundle { get; set; }

    /// <summary>
    /// Address of the linked resource
    /// </summary>
    [HtmlAttributeName(HREF_ATTRIBUTE_NAME)]
    public string Href { get; set; }

    #endregion Properties
}