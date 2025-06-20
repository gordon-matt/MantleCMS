﻿using Extenso.AspNetCore.Mvc.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Mantle.Web.Mvc.Razor.TagHelpers;

/// <summary>
/// Tag helper extensions
/// </summary>
public static class TagHelperExtensions
{
    #region Methods

    /// <summary>
    /// Get string value from tag helper output
    /// </summary>
    /// <param name="output">An information associated with the current HTML tag</param>
    /// <param name="attributeName">Name of the attribute</param>
    /// <returns>Matching name</returns>
    public static async Task<string> GetAttributeValueAsync(this TagHelperOutput output, string attributeName) => string.IsNullOrEmpty(attributeName) || !output.Attributes.TryGetAttribute(attributeName, out var attr)
        ? null
        : attr.Value is string stringValue
            ? stringValue
            : attr.Value switch
            {
                HtmlString htmlString => htmlString.ToString(),
                IHtmlContent content => content.GetString(),
                _ => default
            };

    /// <summary>
    /// Get attributes from tag helper output as collection of key/string value pairs
    /// </summary>
    /// <param name="output">A stateful HTML element used to generate an HTML tag</param>
    /// <returns>Collection of key/string value pairs</returns>
    public static async Task<IDictionary<string, string>> GetAttributeDictionaryAsync(this TagHelperOutput output)
    {
        ArgumentNullException.ThrowIfNull(output);

        var result = new Dictionary<string, string>();

        if (output.Attributes.Count == 0)
        {
            return result;
        }

        foreach (string attrName in output.Attributes.Select(x => x.Name).Distinct())
        {
            result.Add(attrName, await output.GetAttributeValueAsync(attrName));
        }

        return result;
    }

    #endregion Methods
}