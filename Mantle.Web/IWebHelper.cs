namespace Mantle.Web;

public partial interface IWebHelper
{
    string ContentRootPath { get; }

    string WebRootPath { get; }

    bool IsCurrentConnectionSecured();

    string GetRemoteIpAddress();

    string GetUrlHost(bool? useSsl = null);

    string GetUrlReferrer();

    //string MapPath(string path, string basePath = null);

    //void DeleteDirectory(string path);

    void RestartAppDomain();

    bool IsAjaxRequest(HttpRequest request);
}

public partial class WebHelper : IWebHelper
{
    private readonly IHostApplicationLifetime applicationLifetime;
    private readonly IWebHostEnvironment webHostEnvironment;
    private readonly IHttpContextAccessor httpContextAccessor;

    public string ContentRootPath => webHostEnvironment.ContentRootPath;

    public string WebRootPath => webHostEnvironment.WebRootPath;

    public WebHelper(
        IHostApplicationLifetime applicationLifetime,
        IWebHostEnvironment webHostEnvironment,
        IHttpContextAccessor httpContextAccessor)
    {
        this.applicationLifetime = applicationLifetime;
        this.webHostEnvironment = webHostEnvironment;
        this.httpContextAccessor = httpContextAccessor;
    }

    public virtual bool IsCurrentConnectionSecured()
    {
        if (!IsRequestAvailable())
        {
            return false;
        }

        return httpContextAccessor.HttpContext.Request.IsHttps;
    }

    public virtual string GetRemoteIpAddress()
    {
        return httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
    }

    public virtual string GetUrlHost(bool? useSsl = null)
    {
        if (!IsRequestAvailable())
        {
            return string.Empty;
        }

        var hostHeader = httpContextAccessor.HttpContext?.Request?.Headers[HeaderNames.Host];

        if (!hostHeader.HasValue || StringValues.IsNullOrEmpty(hostHeader.Value))
        {
            return string.Empty;
        }

        if (!useSsl.HasValue)
        {
            useSsl = IsCurrentConnectionSecured();
        }

        string host = $"{(useSsl.Value ? Uri.UriSchemeHttps : Uri.UriSchemeHttp)}{Uri.SchemeDelimiter}{hostHeader.Value.FirstOrDefault()}";

        // Ensure that host ends with a slash
        host = $"{host.TrimEnd('/')}/";

        return host;
    }

    public virtual string GetUrlReferrer()
    {
        return httpContextAccessor.HttpContext.Request.Headers[HeaderNames.Referer];
    }

    //public virtual string MapPath(string path, string basePath = null)
    //{
    //    if (string.IsNullOrEmpty(basePath))
    //    {
    //        basePath = WebRootPath;
    //    }

    //    path = path.Replace("~/", string.Empty).TrimStart('/').Replace('/', '\\');
    //    return Path.Combine(basePath, path);
    //}

    ///// <summary>
    /////  Depth-first recursive delete, with handling for descendant directories open in Windows Explorer.
    ///// </summary>
    ///// <param name="path">Directory path</param>
    //public void DeleteDirectory(string path)
    //{
    //    if (string.IsNullOrEmpty(path))
    //        throw new ArgumentNullException(path);

    //    //find more info about directory deletion
    //    //and why we use this approach at https://stackoverflow.com/questions/329355/cannot-delete-directory-with-directory-deletepath-true

    //    foreach (var directory in Directory.GetDirectories(path))
    //    {
    //        DeleteDirectory(directory);
    //    }

    //    try
    //    {
    //        Directory.Delete(path, true);
    //    }
    //    catch (IOException)
    //    {
    //        Directory.Delete(path, true);
    //    }
    //    catch (UnauthorizedAccessException)
    //    {
    //        Directory.Delete(path, true);
    //    }
    //}

    public virtual void RestartAppDomain()
    {
        try
        {
            // TODO: Test in IIS
            applicationLifetime.StopApplication();
        }
        catch
        {
            throw new MantleException(
                $"This site needs to be restarted, but was unable to do so.{Environment.NewLine}Please restart it manually for changes to take effect.");
        }
    }

    protected virtual bool IsRequestAvailable()
    {
        if (httpContextAccessor?.HttpContext == null)
        {
            return false;
        }

        try
        {
            if (httpContextAccessor.HttpContext.Request == null)
            {
                return false;
            }
        }
        catch
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// Gets whether the request is made with AJAX
    /// </summary>
    /// <param name="request">HTTP request</param>
    /// <returns>Result</returns>
    public virtual bool IsAjaxRequest(HttpRequest request)
    {
        if (request == null)
            throw new ArgumentNullException(nameof(request));

        if (request.Headers == null)
            return false;

        return request.Headers["X-Requested-With"] == "XMLHttpRequest";
    }
}