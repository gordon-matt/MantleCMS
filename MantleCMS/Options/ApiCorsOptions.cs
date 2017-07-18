namespace MantleCMS.Options
{
    /// <summary>
    /// A class representing the CORS options for the API. This class cannot be inherited.
    /// </summary>
    public sealed class ApiCorsOptions
    {
        /// <summary>
        /// Gets or sets the names of the HTTP response headers exposed.
        /// </summary>
        public string[] ExposedHeaders { get; set; }

        /// <summary>
        /// Gets or sets the names of the allowed HTTP request headers.
        /// </summary>
        public string[] Headers { get; set; }

        /// <summary>
        /// Gets or sets the allowed HTTP methods.
        /// </summary>
        public string[] Methods { get; set; }

        /// <summary>
        /// Gets or sets the allowed CORS origins.
        /// </summary>
        public string[] Origins { get; set; }
    }
}