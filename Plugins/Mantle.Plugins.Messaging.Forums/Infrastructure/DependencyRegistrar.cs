﻿using Autofac;
using Extenso.AspNetCore.OData;
using Mantle.Localization;
using Mantle.Plugins.Messaging.Forums.Services;
using Mantle.Web.ContentManagement.Infrastructure;
using Mantle.Web.Infrastructure;
using Mantle.Web.Navigation;
using Microsoft.Extensions.Configuration;

namespace Mantle.Plugins.Messaging.Forums.Infrastructure;

public class DependencyRegistrar : IDependencyRegistrar
{
    #region IDependencyRegistrar Members

    public void Register(ContainerBuilder builder, ITypeFinder typeFinder, IConfiguration configuration)
    {
        if (!PluginManager.IsPluginInstalled(Constants.PluginSystemName))
        {
            return;
        }

        builder.RegisterType<DurandalRouteProvider>().As<IDurandalRouteProvider>().SingleInstance();

        // Embedded File Provider
        builder.RegisterType<EmbeddedFileProviderRegistrar>().As<IEmbeddedFileProviderRegistrar>().InstancePerLifetimeScope();

        builder.RegisterType<ForumSettings>().As<ISettings>().InstancePerLifetimeScope();
        builder.RegisterType<LanguagePackInvariant>().As<ILanguagePack>().SingleInstance();

        builder.RegisterType<ForumPermissions>().As<IPermissionProvider>().SingleInstance();
        builder.RegisterType<LocationFormatProvider>().As<ILocationFormatProvider>().SingleInstance();
        builder.RegisterType<NavigationProvider>().As<INavigationProvider>().SingleInstance();
        builder.RegisterType<ODataRegistrar>().As<IODataRegistrar>().SingleInstance();

        builder.RegisterType<AutoMenuProvider>().As<IAutoMenuProvider>().SingleInstance();
        builder.RegisterType<ForumService>().As<IForumService>().InstancePerDependency();
    }

    public int Order => 9999;

    #endregion IDependencyRegistrar Members
}