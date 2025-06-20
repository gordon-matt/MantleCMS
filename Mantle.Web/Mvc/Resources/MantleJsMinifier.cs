﻿using NUglify;
using NUglify.JavaScript;
using WebOptimizer;

namespace Mantle.Web.Mvc.Resources;

/// <summary>
/// Represents a class of processor that handle javascript assets
/// </summary>
/// <remarks>Implementation has taken from WebOptimizer to add logging</remarks>
public partial class MantleJsMinifier : Processor
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
            if (key.EndsWith(".min.js", StringComparison.InvariantCultureIgnoreCase))
            {
                content[key] = context.Content[key];
                continue;
            }

            string input = context.Content[key].AsString();
            var result = Uglify.Js(input, new CodeSettings { TermSemicolons = true });

            string minified = result.Code;

            if (result.HasErrors)
            {
                DependoResolver.Instance.Resolve<ILogger<MantleJsMinifier>>()
                    .LogWarning(new Exception(string.Join(Environment.NewLine, result.Errors)), $"JavaScript minification: {key}");
            }

            content[key] = minified.AsByteArray();
        }

        context.Content = content;

        return;
    }

    #endregion Methods
}