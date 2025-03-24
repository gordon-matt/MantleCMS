using NUglify;
using NUglify.Css;
using WebOptimizer;

namespace Mantle.Web.Mvc.Resources;

/// <summary>
/// Represents a class of processor that handle style assets
/// </summary>
/// <remarks>Implementation has taken from WebOptimizer to add logging</remarks>
public partial class MantleCssMinifier : Processor
{
    #region Methods

    /// <summary>
    /// Executes the processor on the specified configuration.
    /// </summary>
    /// <param name="context">The context used to perform processing to WebOptimizer.IAsset instances</param>
    public override async Task ExecuteAsync(IAssetContext context)
    {
        var content = new Dictionary<string, byte[]>();

        foreach (string key in context.Content.Keys)
        {
            if (key.EndsWith(".min.css", StringComparison.InvariantCultureIgnoreCase))
            {
                content[key] = context.Content[key];
                continue;
            }

            string input = context.Content[key].AsString();
            var result = Uglify.Css(input, new CssSettings());

            string minified = result.Code;

            if (result.HasErrors)
            {
                EngineContext.Current.Resolve<ILogger<MantleCssMinifier>>()
                    .LogWarning(new Exception(string.Join(Environment.NewLine, result.Errors)), $"Stylesheet minification: {key}");
            }

            content[key] = minified.AsByteArray();
        }

        context.Content = content;

        await Task.CompletedTask;
    }

    #endregion Methods
}