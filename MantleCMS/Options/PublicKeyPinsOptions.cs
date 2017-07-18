using System;

namespace MantleCMS.Options
{
    /// <summary>
    /// A class representing the options to use for the <c>Public-Key-Pins</c>
    /// HTTP response header. This class cannot be inherited.
    /// </summary>
    public sealed class PublicKeyPinsOptions
    {
        /// <summary>
        /// Gets or sets the maximum period of time to cache pins for.
        /// </summary>
        public TimeSpan MaxAge { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether to include subdomains.
        /// </summary>
        public bool IncludeSubdomains { get; set; }

        /// <summary>
        /// Gets or sets the SHA-256 hashes of the certificates to pin.
        /// </summary>
        public string[] Sha256Hashes { get; set; }
    }
}