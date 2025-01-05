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
