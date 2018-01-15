using System.Text.RegularExpressions;

namespace Mantle.Web.ContentManagement.Areas.Admin.Media
{
    public static class MediaHelper
    {
        private const string imgTagPattern = @"\<img.*src=""Media.*/>";

        // TinyMCE does not add a "/" at the start of a URL and as such, the relative URLs will sometimes not work
        public static string EnsureCorrectUrls(string html)
        {
            string returnHtml = html;

            var matches = Regex.Matches(html, imgTagPattern);

            foreach (Match match in matches)
            {
                string replacement = match.Value.Replace(@"src=""Media", @"src=""/Media");
                returnHtml = Regex.Replace(returnHtml, Regex.Escape(match.Value), replacement);
            }

            return returnHtml;
        }
    }
}