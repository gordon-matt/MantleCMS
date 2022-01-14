using Autofac;
using Extenso.AspNetCore.OData;
using Mantle.Infrastructure;
using Mantle.Localization;
using Mantle.Messaging;
using Mantle.Messaging.Services;
using Mantle.Security.Membership.Permissions;
using Mantle.Tasks;
using Mantle.Web.Configuration;
using Mantle.Web.Infrastructure;
using Mantle.Web.Messaging.Configuration;
using Mantle.Web.Navigation;

namespace Mantle.Web.Messaging.Infrastructure
{
    public class DependencyRegistrar : IDependencyRegistrar
    {
        #region IDependencyRegistrar Members

        public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
        {
            // Navigation
            builder.RegisterType<AureliaRouteProvider>().As<IAureliaRouteProvider>().SingleInstance();
            builder.RegisterType<NavigationProvider>().As<INavigationProvider>().SingleInstance();

            // Embedded File Provider
            builder.RegisterType<EmbeddedFileProviderRegistrar>().As<IEmbeddedFileProviderRegistrar>().InstancePerLifetimeScope();

            // Configuration
            builder.RegisterType<SmtpSettings>().As<ISettings>().InstancePerLifetimeScope();

            // OData
            builder.RegisterType<ODataRegistrar>().As<IODataRegistrar>().SingleInstance();

            // Services
            builder.RegisterType<MessageService>().As<IMessageService>().InstancePerDependency();
            builder.RegisterType<MessageService>().As<IQueuedMessageProvider>().InstancePerDependency();
            builder.RegisterType<MessageTemplateService>().As<IMessageTemplateService>().InstancePerDependency();
            builder.RegisterType<MessageTemplateVersionService>().As<IMessageTemplateVersionService>().InstancePerDependency();
            builder.RegisterType<QueuedEmailService>().As<IQueuedEmailService>().InstancePerDependency();

            // Tasks
            builder.RegisterType<ProcessQueuedMailTask>().As<ITask>().SingleInstance();

            // Localization
            builder.RegisterType<LanguagePackInvariant>().As<ILanguagePack>().SingleInstance();

            // Security
            builder.RegisterType<MessagingPermissions>().As<IPermissionProvider>().SingleInstance();

            // Other
            builder.RegisterType<Tokenizer>().As<ITokenizer>().InstancePerDependency();

            builder.RegisterType<DefaultMessageTemplateEditor>().As<IMessageTemplateEditor>().SingleInstance();
            builder.RegisterType<GrapesJsMessageTemplateEditor>().As<IMessageTemplateEditor>().SingleInstance();
        }

        public int Order
        {
            get { return 1; }
        }

        #endregion IDependencyRegistrar Members
    }
}