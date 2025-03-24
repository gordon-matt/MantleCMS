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
                new()
                {
                    ModuleId = "viewmodels/admin/localization/languages",
                    Route = "localization/languages",
                    JsPath = "/_content/MantleFramework.Web/js/app/languages",
                    Title = T[MantleWebLocalizableStrings.Localization.Languages]
                },

                new()
                {
                    ModuleId = "viewmodels/admin/localization/localizable-strings",
                    Route = "localization/localizable-strings/:cultureCode",
                    JsPath = "/_content/MantleFramework.Web/js/app/localizable-strings",
                    Title = T[MantleWebLocalizableStrings.Localization.LocalizableStrings]
                },

                //new DurandalRoute
                //{
                //    ModuleId = "viewmodels/admin/log",
                //    Route = "log",
                //    JsPath = "/_content/MantleFramework.Web/js/app/log",
                //    Title = T[MantleWebLocalizableStrings.Log.Title]
                //},

                new()
                {
                    ModuleId = "viewmodels/admin/membership",
                    Route = "membership",
                    JsPath = "/_content/MantleFramework.Web/js/app/membership",
                    Title = T[MantleWebLocalizableStrings.Membership.Title]
                },

                new()
                {
                    ModuleId = "viewmodels/admin/plugins",
                    Route = "plugins",
                    JsPath = "/_content/MantleFramework.Web/js/app/plugins",
                    Title = T[MantleWebLocalizableStrings.Plugins.Title]
                },

                new()
                {
                    ModuleId = "viewmodels/admin/scheduled-tasks",
                    Route = "scheduled-tasks",
                    JsPath = "/_content/MantleFramework.Web/js/app/scheduled-tasks",
                    Title = T[MantleWebLocalizableStrings.ScheduledTasks.Title]
                },

                new()
                {
                    ModuleId = "viewmodels/admin/tenants",
                    Route = "tenants",
                    JsPath = "/_content/MantleFramework.Web/js/app/tenants",
                    Title = T[MantleWebLocalizableStrings.Tenants.Title]
                },

                new()
                {
                    ModuleId = "viewmodels/admin/configuration/settings",
                    Route = "configuration/settings",
                    JsPath = "/_content/MantleFramework.Web/js/app/settings",
                    Title = T[MantleWebLocalizableStrings.General.Settings]
                },

                new()
                {
                    ModuleId = "viewmodels/admin/configuration/themes",
                    Route = "configuration/themes",
                    JsPath = "/_content/MantleFramework.Web/js/app/themes",
                    Title = T[MantleWebLocalizableStrings.General.Themes]
                }
            };

            return routes;
        }
    }
}