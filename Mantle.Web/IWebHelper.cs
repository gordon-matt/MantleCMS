using System;
using System.IO;
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

            path = path.Replace("~/", "").TrimStart('/').Replace('/', '\\');
            return Path.Combine(basePath, path);
        }
    }
}