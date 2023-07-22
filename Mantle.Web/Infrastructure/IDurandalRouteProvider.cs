using Mantle.Infrastructure;
using Microsoft.Extensions.Localization;

namespace Mantle.Web.Infrastructure;

// TODO: Consider exchanging this for a DurandalRouteAttribute instead. Need to test performance though,
//  as it would mean using reflection...
public interface IDurandalRouteProvider
{
    IEnumerable<DurandalRoute> Routes { get; }
}

public class DurandalRouteProvider : IDurandalRouteProvider
{
    public IEnumerable<DurandalRoute> Routes
    {
        get
        {
            var T = EngineContext.Current.Resolve<IStringLocalizer>();
            var routes = new List<DurandalRoute>
            {
                new DurandalRoute
                {
                    ModuleId = "viewmodels/admin/localization/languages",
                    Route = "localization/languages",
                    JsPath = "/durandal-app/embedded/Mantle.Web.Areas.Admin.Localization.Scripts.index",
                    Title = T[MantleWebLocalizableStrings.Localization.Languages]
                },

                new DurandalRoute
                {
                    ModuleId = "viewmodels/admin/localization/localizable-strings",
                    Route = "localization/localizable-strings/:cultureCode",
                    JsPath = "/durandal-app/embedded/Mantle.Web.Areas.Admin.Localization.Scripts.localizable-strings",
                    Title = T[MantleWebLocalizableStrings.Localization.LocalizableStrings]
                },

                new DurandalRoute
                {
                    ModuleId = "viewmodels/admin/log",
                    Route = "log",
                    JsPath = "/durandal-app/embedded/Mantle.Web.Areas.Admin.Log.Scripts.index",
                    Title = T[MantleWebLocalizableStrings.Log.Title]
                },

                new DurandalRoute
                {
                    ModuleId = "viewmodels/admin/membership",
                    Route = "membership",
                    JsPath = "/durandal-app/embedded/Mantle.Web.Areas.Admin.Membership.Scripts.index",
                    Title = T[MantleWebLocalizableStrings.Membership.Title]
                },

                new DurandalRoute
                {
                    ModuleId = "viewmodels/admin/plugins",
                    Route = "plugins",
                    JsPath = "/durandal-app/embedded/Mantle.Web.Areas.Admin.Plugins.Scripts.index",
                    Title = T[MantleWebLocalizableStrings.Plugins.Title]
                },

                new DurandalRoute
                {
                    ModuleId = "viewmodels/admin/scheduledtasks",
                    Route = "scheduledtasks",
                    JsPath = "/durandal-app/embedded/Mantle.Web.Areas.Admin.ScheduledTasks.Scripts.index",
                    Title = T[MantleWebLocalizableStrings.ScheduledTasks.Title]
                },

                new DurandalRoute
                {
                    ModuleId = "viewmodels/admin/tenants",
                    Route = "tenants",
                    JsPath = "/durandal-app/embedded/Mantle.Web.Areas.Admin.Tenants.Scripts.index",
                    Title = T[MantleWebLocalizableStrings.Tenants.Title]
                },

                new DurandalRoute
                {
                    ModuleId = "viewmodels/admin/configuration/settings",
                    Route = "configuration/settings",
                    JsPath = "/durandal-app/embedded/Mantle.Web.Areas.Admin.Configuration.Scripts.settings",
                    Title = T[MantleWebLocalizableStrings.General.Settings]
                },

                new DurandalRoute
                {
                    ModuleId = "viewmodels/admin/configuration/themes",
                    Route = "configuration/themes",
                    JsPath = "/durandal-app/embedded/Mantle.Web.Areas.Admin.Configuration.Scripts.themes",
                    Title = T[MantleWebLocalizableStrings.General.Themes]
                }
            };

            return routes;
        }
    }
}