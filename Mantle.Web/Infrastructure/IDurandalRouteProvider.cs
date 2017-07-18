using System.Collections.Generic;
using Mantle.Infrastructure;
using Mantle.Web.Mvc.EmbeddedResources;
using Microsoft.Extensions.Localization;

namespace Mantle.Web.Infrastructure
{
    // TODO: Consider exchanging this for a DurandalRouteAttribute instead. Need to test performance though,
    //  as it would mean using reflection...
    public interface IDurandalRouteProvider
    {
        string Area { get; }

        IEnumerable<DurandalRoute> Routes { get; }
    }

    public class DurandalRouteProvider : IDurandalRouteProvider
    {
        #region IDurandalRouteProvider Members

        public string Area
        {
            get { return MantleWebConstants.Areas.Admin; }
        }

        public IEnumerable<DurandalRoute> Routes
        {
            get
            {
                var T = EngineContext.Current.Resolve<IStringLocalizer>();
                var routes = new List<DurandalRoute>();

                //routes.Add(new DurandalRoute
                //{
                //    ModuleId = "viewmodels/admin/indexing",
                //    Route = "indexing",
                //    JsPath = scriptRegister.GetBundleUrl("kore-web/indexing"),
                //    Title = T[MantleWebLocalizableStrings.Indexing.Title]
                //});

                routes.Add(new DurandalRoute
                {
                    ModuleId = "viewmodels/admin/membership",
                    Route = "membership",
                    JsPath = "/Mantle.Web.Areas.Admin.Membership.Scripts.membership",
                    Title = T[MantleWebLocalizableStrings.Membership.Title]
                });

                routes.Add(new DurandalRoute
                {
                    ModuleId = "viewmodels/admin/localization/languages",
                    Route = "localization/languages",
                    JsPath = "/Mantle.Web.Areas.Admin.Localization.Scripts.index",
                    Title = T[MantleWebLocalizableStrings.Localization.Languages]
                });

                routes.Add(new DurandalRoute
                {
                    ModuleId = "viewmodels/admin/localization/localizable-strings",
                    Route = "localization/localizable-strings/:cultureCode",
                    JsPath = "/Mantle.Web.Areas.Admin.Localization.Scripts.localizableStrings",
                    Title = T[MantleWebLocalizableStrings.Localization.LocalizableStrings]
                });

                routes.Add(new DurandalRoute
                {
                    ModuleId = "viewmodels/admin/log",
                    Route = "log",
                    JsPath = "/Mantle.Web.Areas.Admin.Log.Scripts.index",
                    Title = T[MantleWebLocalizableStrings.Log.Title]
                });

                //routes.Add(new DurandalRoute
                //{
                //    ModuleId = "viewmodels/admin/plugins",
                //    Route = "plugins",
                //    JsPath = scriptRegister.GetBundleUrl("kore-web/plugins"),
                //    Title = T[MantleWebLocalizableStrings.Plugins.Title]
                //});

                routes.Add(new DurandalRoute
                {
                    ModuleId = "viewmodels/admin/scheduled-tasks",
                    Route = "scheduled-tasks",
                    JsPath = "/Mantle.Web.Areas.Admin.ScheduledTasks.Scripts.index",
                    Title = T[MantleWebLocalizableStrings.ScheduledTasks.Title]
                });

                routes.Add(new DurandalRoute
                {
                    ModuleId = "viewmodels/admin/tenants",
                    Route = "tenants",
                    JsPath = "/Mantle.Web.Areas.Admin.Tenants.Scripts.index",
                    Title = T[MantleWebLocalizableStrings.Tenants.Title]
                });

                routes.Add(new DurandalRoute
                {
                    ModuleId = "viewmodels/admin/configuration/settings",
                    Route = "configuration/settings",
                    JsPath = "/Mantle.Web.Areas.Admin.Configuration.Scripts.settings",
                    Title = T[MantleWebLocalizableStrings.General.Settings]
                });

                routes.Add(new DurandalRoute
                {
                    ModuleId = "viewmodels/admin/configuration/themes",
                    Route = "configuration/themes",
                    JsPath = "/Mantle.Web.Areas.Admin.Configuration.Scripts.themes",
                    Title = T[MantleWebLocalizableStrings.General.Themes]
                });

                return routes;
            }
        }

        #endregion IDurandalRouteProvider Members
    }
}