using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Mantle.Web
{
    public static class StringExtensions
    {
        public static string ToSlugUrl(this string value)
        {
            string stringFormKd = value.Normalize(NormalizationForm.FormKD);
            var stringBuilder = new StringBuilder();

            foreach (char character in stringFormKd)
            {
                UnicodeCategory unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(character);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(character);
                }
            }

            // Replace some characters
            stringBuilder
                .Replace("!", "-")
                .Replace("$", "-")
                .Replace("&", "-")
                .Replace("\"", "-")
                .Replace("'", "-")
                .Replace("(", "-")
                .Replace(")", "-")
                .Replace("*", "-")
                .Replace("+", "-")
                .Replace(",", "-")
                .Replace(";", "-")
                .Replace(".", "-")
                .Replace("/", "-")
                .Replace("\\", "-")
                .Replace(":", "-")
                .Replace("=", "-")
                .Replace("?", "-")
                .Replace("&", "-")
                .Replace("_", "-")
                .Replace("~", "-")
                .Replace("`", "-")
                .Replace("#", "-")
                .Replace("%", "-")
                .Replace("^", "-")
                .Replace("[", "-")
                .Replace("]", "-")
                .Replace("{", "-")
                .Replace("}", "-")
                .Replace("|", "-")
                .Replace("<", "-")
                .Replace(">", "-");

            var slug = stringBuilder.ToString().Normalize(NormalizationForm.FormKC);

            //First to lower case
            slug = slug.ToLowerInvariant();

            if (!slug.IsRightToLeft())
            {
                //Remove all accents
                var bytes = Encoding.GetEncoding("Cyrillic").GetBytes(slug);
                slug = Encoding.ASCII.GetString(bytes);

                //Remove invalid chars
                slug = Regex.Replace(slug, @"[^a-z0-9\s-_]", string.Empty, RegexOptions.Compiled);
            }

            //Replace spaces
            slug = Regex.Replace(slug, @"\s", "-", RegexOptions.Compiled);

            //Trim dashes from end
            slug = slug.Trim('-', '_');

            //Replace double occurences of - or _
            slug = Regex.Replace(slug, @"([-_]){2,}", "$1", RegexOptions.Compiled);

            return slug;
        }
    }
}