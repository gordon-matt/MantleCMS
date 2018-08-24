using System;
using System.IO;
using Mantle.Exceptions;
using Mantle.Helpers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Mantle.Web
{
    public partial interface IWebHelper
    {
        string ContentRootPath { get; }

        string WebRootPath { get; }

        bool IsCurrentConnectionSecured();

        string GetRemoteIpAddress();

        string GetUrlHost();

        string GetUrlReferrer();

        //string MapPath(string path, string basePath = null);

        //void DeleteDirectory(string path);

        void RestartAppDomain();
    }

    public partial class WebHelper : IWebHelper
    {
        private readonly IApplicationLifetime applicationLifetime;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly IHttpContextAccessor httpContextAccessor;

        public string ContentRootPath => hostingEnvironment.ContentRootPath;

        public string WebRootPath => hostingEnvironment.WebRootPath;

        public WebHelper(
            IApplicationLifetime applicationLifetime,
            IHostingEnvironment hostingEnvironment,
            IHttpContextAccessor httpContextAccessor)
        {
            this.applicationLifetime = applicationLifetime;
            this.hostingEnvironment = hostingEnvironment;
            this.httpContextAccessor = httpContextAccessor;
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

        public virtual string GetRemoteIpAddress()
        {
            return httpContextAccessor.HttpContext.Connection.RemoteIpAddress.ToString();
        }

        public virtual string GetUrlHost()
        {
            return httpContextAccessor.HttpContext.Request.Host.Host;
        }

        public virtual string GetUrlReferrer()
        {
            return httpContextAccessor.HttpContext.Request.Headers["Referer"];
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
    }
}