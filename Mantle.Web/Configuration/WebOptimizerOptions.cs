namespace Mantle.Web.Configuration;

public partial class WebOptimizerOptions : WebOptimizer.WebOptimizerOptions
{
    #region Ctor

    public WebOptimizerOptions()
    {
        EnableDiskCache = true;
        EnableTagHelperBundling = false;
    }

    #endregion Ctor

    #region Properties

    /// <summary>
    /// A value indicating whether JS file bundling and minification is enabled
    /// </summary>
    public bool EnableJavaScriptBundling { get; protected set; } = true;

    /// <summary>
    /// A value indicating whether CSS file bundling and minification is enabled
    /// </summary>
    public bool EnableCssBundling { get; protected set; } = true;

    /// <summary>
    /// Gets or sets a suffix for the js-file name of generated bundles
    /// </summary>
    public string JavaScriptBundleSuffix { get; protected set; } = ".scripts";

    /// <summary>
    /// Gets or sets a suffix for the css-file name of generated bundles
    /// </summary>
    public string CssBundleSuffix { get; protected set; } = ".styles";

    #endregion Properties
}