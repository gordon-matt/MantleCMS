using Autofac;
using Mantle.Data.Entity;
using Mantle.Data.Services;
using Mantle.Localization;
using Mantle.Security.Membership;
using Mantle.Web.CommonResources.ScriptBuilder.Toasts;
using Mantle.Web.Infrastructure;
using Mantle.Web.Navigation;
using MantleCMS.Areas.Admin;
using MantleCMS.Infrastructure.ScriptBuilder;
using MantleCMS.Services;

namespace MantleCMS.Infrastructure;

public class DependencyRegistrar : IDependencyRegistrar
{
    public int Order => 1;

    public void Register(ContainerBuilder builder, ITypeFinder typeFinder, IConfiguration configuration)
    {
        //builder.RegisterType<SqlDbHelper>().As<IMantleDbHelper>().SingleInstance();

        builder.RegisterType<ApplicationDbContextFactory>().As<IDbContextFactory>().SingleInstance();

        builder.RegisterGeneric(typeof(MantleEntityFrameworkRepository<>))
            .As(typeof(IRepository<>))
            .InstancePerLifetimeScope();

        builder.RegisterType<SqlServerEntityFrameworkHelper>().As<IMantleEntityFrameworkHelper>().SingleInstance();

        // SPA Routes
        builder.RegisterType<AdminDurandalRouteProvider>().As<IDurandalRouteProvider>().SingleInstance();

        // Services
        builder.RegisterType<MembershipService>().As<IMembershipService>().InstancePerDependency();

        // Localization
        builder.RegisterType<LanguagePackInvariant>().As<ILanguagePack>().InstancePerDependency();

        // Navigation
        builder.RegisterType<AdminNavigationProvider>().As<INavigationProvider>().SingleInstance();

        // Script Builders..
        builder.RegisterType<ToastyScriptBuilder>().As<IToastsScriptBuilder>().SingleInstance();
    }
}