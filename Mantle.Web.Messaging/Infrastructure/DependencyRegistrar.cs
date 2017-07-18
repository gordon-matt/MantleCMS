using Autofac;
using Mantle.Infrastructure;
using Mantle.Localization;
using Mantle.Web.Security.Membership.Permissions;

namespace Mantle.Web.Messaging.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar<ContainerBuilder>
    {
        #region IDependencyRegistrar Members

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            builder.RegisterType<LanguagePackInvariant>().As<ILanguagePack>().SingleInstance();
            builder.RegisterType<MessagingPermissions>().As<IPermissionProvider>().SingleInstance();
        }

        public int Order
        {
            get { return 1; }
        }

        #endregion IDependencyRegistrar Members
    }
}