using System;

namespace MantleCMS.Options
{
    /// <summary>
    /// A class representing the external link options for the site. This class cannot be inherited.
    /// </summary>
    public sealed class ExternalLinksOptions
    {
        /// <summary>
        /// Gets or sets the URI of the blog.
        /// </summary>
        public Uri Blog { get; set; }

        /// <summary>
        /// Gets or sets the URI of the status website.
        /// </summary>
        public Uri Status { get; set; }

        /// <summary>
        /// Gets or sets the options for the URIs to use for reports.
        /// </summary>
        public ReportOptions Reports { get; set; }
    }
}