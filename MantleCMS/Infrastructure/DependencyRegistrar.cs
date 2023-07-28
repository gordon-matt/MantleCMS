using Autofac;
using Mantle.Data.Entity;
using Mantle.Data.Services;
using Mantle.Localization;
using Mantle.Security.Membership;
using Mantle.Web.Infrastructure;
using Mantle.Web.Navigation;
using MantleCMS.Areas.Admin;
using MantleCMS.Services;

namespace MantleCMS.Infrastructure;

public class DependencyRegistrar : IDependencyRegistrar
{
    public int Order
    {
        get { return 1; }
    }

    public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
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
    }
}