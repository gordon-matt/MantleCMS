using Autofac;
using Mantle.Data;
using Mantle.Data.Entity.EntityFramework;
using Mantle.Infrastructure;
using Mantle.Localization;
using Mantle.Security.Membership;
using Mantle.Web.Infrastructure;
using Mantle.Web.Navigation;
using MantleCMS.Areas.Admin;
using MantleCMS.Data;
using MantleCMS.Services;

namespace MantleCMS.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar<ContainerBuilder>
    {
        public int Order
        {
            get { return 1; }
        }

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            //builder.RegisterType<SqlDbHelper>().As<IMantleDbHelper>().SingleInstance();
            builder.RegisterType<SqlEntityFrameworkHelper>().As<IEntityFrameworkHelper>().InstancePerDependency();

            builder.RegisterType<ApplicationDbContextFactory>().As<IDbContextFactory>().SingleInstance();

            builder.RegisterGeneric(typeof(EntityFrameworkRepository<>))
                .As(typeof(IRepository<>))
                .InstancePerLifetimeScope();

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
}