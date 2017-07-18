namespace MantleCMS.Options
{
    /// <summary>
    /// A class representing the license options for the API. This class cannot be inherited.
    /// </summary>
    public sealed class LicenseOptions
    {
        /// <summary>
        /// Gets or sets the license name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the license URL.
        /// </summary>
        public string Url { get; set; }
    }
}