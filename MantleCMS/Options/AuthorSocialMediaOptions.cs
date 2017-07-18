namespace MantleCMS.Options
{
    /// <summary>
    /// A class representing the social media options for the site author. This class cannot be inherited.
    /// </summary>
    public sealed class AuthorSocialMediaOptions
    {
        /// <summary>
        /// Gets or sets the Facebook profile Id of the author.
        /// </summary>
        public string Facebook { get; set; }

        /// <summary>
        /// Gets or sets the Twitter handle of the author.
        /// </summary>
        public string Twitter { get; set; }
    }
}