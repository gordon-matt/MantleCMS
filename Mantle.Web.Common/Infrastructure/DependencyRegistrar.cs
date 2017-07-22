﻿using Autofac;
using Mantle.Infrastructure;
using Mantle.Localization;
using Mantle.Web.Common.Areas.Admin.Regions.Services;
using Mantle.Web.Infrastructure;
using Mantle.Web.Mvc.Themes;
using Mantle.Web.Navigation;
using Mantle.Web.Security.Membership.Permissions;

namespace Mantle.Web.Common.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar<ContainerBuilder>
    {
        #region IDependencyRegistrar Members

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            builder.RegisterType<DurandalRouteProvider>().As<IDurandalRouteProvider>().SingleInstance();
            builder.RegisterType<RequireJSConfigProvider>().As<IRequireJSConfigProvider>().SingleInstance();

            builder.RegisterType<LanguagePackInvariant>().As<ILanguagePack>().SingleInstance();
            builder.RegisterType<NavigationProvider>().As<INavigationProvider>().SingleInstance();
            builder.RegisterType<LocationFormatProvider>().As<ILocationFormatProvider>().SingleInstance();
            builder.RegisterType<RegionService>().As<IRegionService>().InstancePerDependency();
            builder.RegisterType<RegionSettingsService>().As<IRegionSettingsService>().InstancePerDependency();
            builder.RegisterType<Permissions>().As<IPermissionProvider>().SingleInstance();
        }

        public int Order
        {
            get { return 1; }
        }

        #endregion IDependencyRegistrar Members
    }
}