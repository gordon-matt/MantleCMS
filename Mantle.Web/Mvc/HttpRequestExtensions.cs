namespace Mantle.Web.Mvc;

public static class HttpRequestExtensions
{
    extension(HttpRequest request)
    {
        /// <summary>
        /// Determines whether the specified HTTP request is an AJAX request.
        /// </summary>
        /// <returns>true if the specified HTTP request is an AJAX request; otherwise, false.</returns>
        /// <exception cref="T:System.ArgumentNullException">The <paramref name="request"/> parameter is null (Nothing in Visual Basic).</exception>
        public bool IsAjaxRequest() => request is null
            ? throw new ArgumentNullException("request")
            : request.Headers is not null && request.Headers.XRequestedWith == "XMLHttpRequest";

        public string UrlReferer() => request.Headers.Referer.ToString();
    }
}