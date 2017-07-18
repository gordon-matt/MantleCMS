using System;

namespace MantleCMS.Options
{
    /// <summary>
    /// A class representing the options for reports. This class cannot be inherited.
    /// </summary>
    public sealed class ReportOptions
    {
        /// <summary>
        /// Gets or sets the URI to use for <c>Content-Security-Policy</c>.
        /// </summary>
        public Uri ContentSecurityPolicy { get; set; }

        /// <summary>
        /// Gets or sets the URI to use for <c>Content-Security-Policy-Report-Only</c>.
        /// </summary>
        public Uri ContentSecurityPolicyReportOnly { get; set; }

        /// <summary>
        /// Gets or sets the URI to use for <c>Public-Key-Pins</c>.
        /// </summary>
        public Uri PublicKeyPins { get; set; }

        /// <summary>
        /// Gets or sets the URI to use for <c>Public-Key-Pins-Report-Only</c>.
        /// </summary>
        public Uri PublicKeyPinsReportOnly { get; set; }
    }
}