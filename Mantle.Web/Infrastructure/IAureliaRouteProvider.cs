using System.Collections.Generic;
using Mantle.Infrastructure;
using Microsoft.Extensions.Localization;

namespace Mantle.Web.Infrastructure
{
    // TODO: Consider exchanging this for a AureliaRouteAttribute instead. Need to test performance though,
    //  as it would mean using reflection...
    public interface IAureliaRouteProvider
    {
        string Area { get; }

        IEnumerable<AureliaRoute> Routes { get; }

        IDictionary<string, string> ModuleIdToViewUrlMappings { get; }
    }

    public class AureliaRouteProvider : IAureliaRouteProvider
    {
        #region IAureliaRouteProvider Members

        public string Area => MantleWebConstants.Areas.Admin;

        public IEnumerable<AureliaRoute> Routes
        {
            get
            {
                var T = EngineContext.Current.Resolve<IStringLocalizer>();
                var routes = new List<AureliaRoute>();

                //routes.Add(new AureliaRoute
                //{
                //    ModuleId = "/aurelia-app/embedded/Mantle.Web.Areas.Admin.Membership.Scripts.membership",
                //    Route = "membership",
                //    Title = T[MantleWebLocalizableStrings.Membership.Title]
                //});

                //routes.Add(new AureliaRoute
                //{
                //    ModuleId = "/aurelia-app/embedded/Mantle.Web.Areas.Admin.Localization.Scripts.index",
                //    Route = "localization/languages",
                //    Title = T[MantleWebLocalizableStrings.Localization.Languages]
                //});

                //routes.Add(new AureliaRoute
                //{
                //    ModuleId = "/aurelia-app/embedded/Mantle.Web.Areas.Admin.Localization.Scripts.localizableStrings",
                //    Route = "localization/localizable-strings/:cultureCode",
                //    Title = T[MantleWebLocalizableStrings.Localization.LocalizableStrings]
                //});

                //routes.Add(new AureliaRoute
                //{
                //    ModuleId = "/aurelia-app/embedded/Mantle.Web.Areas.Admin.Log.Scripts.index",
                //    Route = "log",
                //    Title = T[MantleWebLocalizableStrings.Log.Title]
                //});

                //routes.Add(new AureliaRoute
                //{
                //    ModuleId = "/aurelia-app/embedded/Mantle.Web.Areas.Admin.Plugins.Scripts.index",
                //    Route = "plugins",
                //    Title = T[MantleWebLocalizableStrings.Plugins.Title]
                //});

                //routes.Add(new AureliaRoute
                //{
                //    ModuleId = "/aurelia-app/embedded/Mantle.Web.Areas.Admin.ScheduledTasks.Scripts.index",
                //    Route = "scheduled-tasks",
                //    Title = T[MantleWebLocalizableStrings.ScheduledTasks.Title]
                //});

                routes.Add(new AureliaRoute
                {
                    ModuleId = "/aurelia-app/embedded/Mantle.Web.Areas.Admin.Tenants.Scripts.index",
                    Route = "tenants",
                    Title = T[MantleWebLocalizableStrings.Tenants.Title]
                });

                //routes.Add(new AureliaRoute
                //{
                //    ModuleId = "/aurelia-app/embedded/Mantle.Web.Areas.Admin.Configuration.Scripts.settings",
                //    Route = "configuration/settings",
                //    Title = T[MantleWebLocalizableStrings.General.Settings]
                //});

                //routes.Add(new AureliaRoute
                //{
                //    ModuleId = "/aurelia-app/embedded/Mantle.Web.Areas.Admin.Configuration.Scripts.themes",
                //    Route = "configuration/themes",
                //    Title = T[MantleWebLocalizableStrings.General.Themes]
                //});

                return routes;
            }
        }

        public IDictionary<string, string> ModuleIdToViewUrlMappings => new Dictionary<string, string>
        {
            //{ "aurelia-app/embedded/Mantle.Web.Areas.Admin.Membership.Scripts.membership", "admin/membership" },
            //{ "aurelia-app/embedded/Mantle.Web.Areas.Admin.Localization.Scripts.index", "admin/localization/languages" },
            //{ "aurelia-app/embedded/Mantle.Web.Areas.Admin.Localization.Scripts.localizableStrings", "admin/localization/localizable-strings" },
            //{ "aurelia-app/embedded/Mantle.Web.Areas.Admin.Log.Scripts.index", "admin/log" },
            //{ "aurelia-app/embedded/Mantle.Web.Areas.Admin.Plugins.Scripts.index", "admin/plugins" },
            //{ "aurelia-app/embedded/Mantle.Web.Areas.Admin.ScheduledTasks.Scripts.index", "admin/scheduled-tasks" },
            { "aurelia-app/embedded/Mantle.Web.Areas.Admin.Tenants.Scripts.index", "admin/tenants" },
            //{ "aurelia-app/embedded/Mantle.Web.Areas.Admin.Configuration.Scripts.settings", "admin/configuration/settings" },
            //{ "aurelia-app/embedded/Mantle.Web.Areas.Admin.Configuration.Scripts.themes", "admin/configuration/themes" },
        };

        #endregion IAureliaRouteProvider Members
    }
}