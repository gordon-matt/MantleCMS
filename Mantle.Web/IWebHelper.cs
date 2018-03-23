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
            bool success = TryWriteWebConfig();
            if (!success)
            {
                throw new MantleException("Mantle needs to be restarted due to a configuration change, but was unable to do so." + Environment.NewLine +
                    "To prevent this issue in the future, a change to the web server configuration is required:" + Environment.NewLine +
                    "- run the application in a full trust environment, or" + Environment.NewLine +
                    "- give the application write access to the 'web.config' file.");
            }
        }

        private bool TryWriteWebConfig()
        {
            try
            {
                // In medium trust, "UnloadAppDomain" is not supported. Touch web.config
                // to force an AppDomain restart.
                File.SetLastWriteTimeUtc(CommonHelper.MapPath("~/web.config", ContentRootPath), DateTime.UtcNow);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}