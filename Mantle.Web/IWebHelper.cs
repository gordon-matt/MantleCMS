using System;
using System.IO;
using Mantle.Exceptions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Mantle.Web
{
    public partial interface IWebHelper
    {
        string ContentRootPath { get; }

        string WebRootPath { get; }

        string GetUrlReferrer();

        string GetRemoteIpAddress();

        bool IsCurrentConnectionSecured();

        string GetUrlHost();

        string MapPath(string path, string basePath = null);

        void RestartSite();
    }

    public partial class WebHelper : IWebHelper
    {
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly IHttpContextAccessor httpContextAccessor;

        public string ContentRootPath
        {
            get { return hostingEnvironment.ContentRootPath; }
        }

        public string WebRootPath
        {
            get { return hostingEnvironment.WebRootPath; }
        }

        public WebHelper(
            IHostingEnvironment hostingEnvironment,
            IHttpContextAccessor httpContextAccessor)
        {
            this.hostingEnvironment = hostingEnvironment;
            this.httpContextAccessor = httpContextAccessor;
        }

        public virtual string GetUrlReferrer()
        {
            return httpContextAccessor.HttpContext.Request.Headers["Referer"];
        }

        public virtual string GetRemoteIpAddress()
        {
            return httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
        }

        public virtual bool IsCurrentConnectionSecured()
        {
            bool useSsl = false;

            var httpContext = httpContextAccessor.HttpContext;

            if (httpContext != null && httpContext.Request != null)
            {
                useSsl = httpContext.Request.IsHttps;
            }

            return useSsl;
        }

        public virtual string GetUrlHost()
        {
            return httpContextAccessor.HttpContext.Request.Host.Host;
        }

        public virtual string MapPath(string path, string basePath = null)
        {
            if (string.IsNullOrEmpty(basePath))
            {
                basePath = WebRootPath;
            }

            path = path.Replace("~/", string.Empty).TrimStart('/').Replace('/', '\\');
            return Path.Combine(basePath, path);
        }

        public virtual void RestartSite()
        {
            bool success = TryWriteWebConfig();
            if (!success)
            {
                throw new MantleException("Mantle needs to be restarted due to a configuration change, but was unable to do so." + Environment.NewLine +
                    "To prevent this issue in the future, a change to the web server configuration is required:" + Environment.NewLine +
                    "- run the application in a full trust environment, or" + Environment.NewLine +
                    "- give the application write access to the 'web.config' file.");
            }

            //if (GetTrustLevel() > AspNetHostingPermissionLevel.Medium)
            //{
            //    //full trust
            //    HttpRuntime.UnloadAppDomain();

            //    TryWriteGlobalAsax();
            //}
            //else
            //{
            //    //medium trust
            //    bool success = TryWriteWebConfig();
            //    if (!success)
            //    {
            //        throw new MantleException("Mantle needs to be restarted due to a configuration change, but was unable to do so." + Environment.NewLine +
            //            "To prevent this issue in the future, a change to the web server configuration is required:" + Environment.NewLine +
            //            "- run the application in a full trust environment, or" + Environment.NewLine +
            //            "- give the application write access to the 'web.config' file.");
            //    }

            //    success = TryWriteGlobalAsax();
            //    if (!success)
            //    {
            //        throw new MantleException("Mantle needs to be restarted due to a configuration change, but was unable to do so." + Environment.NewLine +
            //            "To prevent this issue in the future, a change to the web server configuration is required:" + Environment.NewLine +
            //            "- run the application in a full trust environment, or" + Environment.NewLine +
            //            "- give the application write access to the 'Global.asax' file.");
            //    }
            //}

            //// If setting up extensions/modules requires an AppDomain restart, it's very unlikely the
            //// current request can be processed correctly.  So, we redirect to the same URL, so that the
            //// new request will come to the newly started AppDomain.
            //if (httpContext != null && makeRedirect)
            //{
            //    if (string.IsNullOrEmpty(redirectUrl))
            //        redirectUrl = GetThisPageUrl(true);
            //    httpContext.Response.Redirect(redirectUrl, true /*endResponse*/);
            //}
        }

        private bool TryWriteWebConfig()
        {
            try
            {
                // In medium trust, "UnloadAppDomain" is not supported. Touch web.config
                // to force an AppDomain restart.
                File.SetLastWriteTimeUtc(MapPath("~/web.config", ContentRootPath), DateTime.UtcNow);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}