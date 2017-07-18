namespace MantleCMS.Options
{
    /// <summary>
    /// A class representing the API options for the site. This class cannot be inherited.
    /// </summary>
    public sealed class ApiOptions
    {
        /// <summary>
        /// Gets or sets the CORS options for the API.
        /// </summary>
        public ApiCorsOptions Cors { get; set; }

        /// <summary>
        /// Gets or sets the documentation options for the API.
        /// </summary>
        public DocumentationOptions Documentation { get; set; }

        /// <summary>
        /// Gets or sets the license options for the API.
        /// </summary>
        public LicenseOptions License { get; set; }
    }
}