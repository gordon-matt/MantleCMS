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

    public void Register(IContainerBuilder builder, ITypeFinder typeFinder, IConfiguration configuration)
    {
        //builder.Register<IMantleDbHelper, SqlDbHelper>(ServiceLifetime.Singleton);

        builder.Register<IDbContextFactory, ApplicationDbContextFactory>(ServiceLifetime.Singleton);

        builder.RegisterGeneric(typeof(IRepository<>), typeof(MantleEntityFrameworkRepository<>), ServiceLifetime.Scoped);

        builder.Register<IMantleEntityFrameworkHelper, SqlServerEntityFrameworkHelper>(ServiceLifetime.Singleton);

        // SPA Routes
        builder.Register<IDurandalRouteProvider, AdminDurandalRouteProvider>(ServiceLifetime.Singleton);

        // Services
        builder.Register<IMembershipService, MembershipService>(ServiceLifetime.Transient);

        // Localization
        builder.Register<ILanguagePack, LanguagePackInvariant>(ServiceLifetime.Transient);

        // Navigation
        builder.Register<INavigationProvider, AdminNavigationProvider>(ServiceLifetime.Singleton);

        // Script Builders..
        builder.Register<IToastsScriptBuilder, ToastyScriptBuilder>(ServiceLifetime.Singleton);
    }
}