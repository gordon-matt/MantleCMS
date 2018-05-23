using System;
using Mantle.Infrastructure;
using Mantle.Security.Membership.Permissions;
using Mantle.Web.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;

namespace Mantle.Web.Mvc
{
    public class MantleController : Controller
    {
        public ILogger Logger { get; private set; }

        public IStringLocalizer T { get; private set; }

        public IWorkContext WorkContext { get; private set; }

        public Lazy<SiteSettings> SiteSettings { get; private set; }

        protected MantleController()
        {
            WorkContext = EngineContext.Current.Resolve<IWorkContext>();
            T = EngineContext.Current.Resolve<IStringLocalizer>();
            var loggerFactory = EngineContext.Current.Resolve<ILoggerFactory>();
            Logger = loggerFactory.CreateLogger(GetType());
            SiteSettings = new Lazy<SiteSettings>(() => EngineContext.Current.Resolve<SiteSettings>());
        }

        protected virtual bool CheckPermission(Permission permission)
        {
            if (permission == null)
            {
                return true;
            }

            var authorizationService = EngineContext.Current.Resolve<IAuthorizationService>();
            return authorizationService.TryCheckAccess(permission, WorkContext.CurrentUser);
        }

        protected virtual IActionResult RedirectToHomePage()
        {
            return RedirectToAction("Index", "Home", new { area = string.Empty });
        }
    }
}