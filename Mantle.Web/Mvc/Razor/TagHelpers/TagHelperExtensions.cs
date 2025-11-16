using Extenso.AspNetCore.Mvc.Html;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Mantle.Web.Mvc.Razor.TagHelpers;

/// <summary>
/// Tag helper extensions
/// </summary>
public static class TagHelperExtensions
{
    extension(TagHelperOutput output)
    {
        /// <summary>
        /// Get string value from tag helper output
        /// </summary>
        /// <param name="attributeName">Name of the attribute</param>
        /// <returns>Matching name</returns>
        public async Task<string> GetAttributeValueAsync(string attributeName) => string.IsNullOrEmpty(attributeName) || !output.Attributes.TryGetAttribute(attributeName, out var attr)
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
        /// <returns>Collection of key/string value pairs</returns>
        public async Task<IDictionary<string, string>> GetAttributeDictionaryAsync()
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
    }
}