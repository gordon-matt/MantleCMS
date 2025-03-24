namespace Mantle.Web.Mvc;

public static class HttpRequestExtensions
{
    /// <summary>
    /// Determines whether the specified HTTP request is an AJAX request.
    /// </summary>
    ///
    /// <returns>
    /// true if the specified HTTP request is an AJAX request; otherwise, false.
    /// </returns>
    /// <param name="request">The HTTP request.</param><exception cref="T:System.ArgumentNullException">The <paramref name="request"/> parameter is null (Nothing in Visual Basic).</exception>
    public static bool IsAjaxRequest(this HttpRequest request) => request == null
        ? throw new ArgumentNullException("request")
        : request.Headers != null && request.Headers["X-Requested-With"] == "XMLHttpRequest";

    public static string UrlReferer(this HttpRequest request) => request.Headers["Referer"].ToString();
}