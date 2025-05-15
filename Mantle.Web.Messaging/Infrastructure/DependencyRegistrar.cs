using Extenso.AspNetCore.OData;
using Mantle.Localization;
using Mantle.Messaging;
using Mantle.Messaging.Services;
using Mantle.Tasks;
using Mantle.Web.Infrastructure;
using Mantle.Web.Messaging.Configuration;
using Mantle.Web.Navigation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mantle.Web.Messaging.Infrastructure;

public class DependencyRegistrar : IDependencyRegistrar
{
    #region IDependencyRegistrar Members

    public void Register(IContainerBuilder builder, ITypeFinder typeFinder, IConfiguration configuration)
    {
        // Navigation
        builder.Register<IDurandalRouteProvider, DurandalRouteProvider>(ServiceLifetime.Singleton);
        builder.Register<INavigationProvider, NavigationProvider>(ServiceLifetime.Singleton);

        // Embedded File Provider
        builder.Register<IEmbeddedFileProviderRegistrar, EmbeddedFileProviderRegistrar>(ServiceLifetime.Scoped);

        // Configuration
        builder.Register<ISettings, SmtpSettings>(ServiceLifetime.Scoped);

        // OData
        builder.Register<IODataRegistrar, ODataRegistrar>(ServiceLifetime.Singleton);

        // Services
        builder.Register<IMessageService, MessageService>(ServiceLifetime.Transient);
        builder.Register<IQueuedMessageProvider, MessageService>(ServiceLifetime.Transient);
        builder.Register<IMessageTemplateService, MessageTemplateService>(ServiceLifetime.Transient);
        builder.Register<IMessageTemplateVersionService, MessageTemplateVersionService>(ServiceLifetime.Transient);
        builder.Register<IQueuedEmailService, QueuedEmailService>(ServiceLifetime.Transient);

        // Tasks
        builder.Register<ITask, ProcessQueuedMailTask>(ServiceLifetime.Singleton);

        // Localization
        builder.Register<ILanguagePack, LanguagePackInvariant>(ServiceLifetime.Singleton);

        // Security
        builder.Register<IPermissionProvider, MessagingPermissions>(ServiceLifetime.Singleton);

        // Other
        builder.Register<ITokenizer, Tokenizer>(ServiceLifetime.Transient);

        builder.Register<IMessageTemplateEditor, DefaultMessageTemplateEditor>(ServiceLifetime.Singleton);
        builder.Register<IMessageTemplateEditor, GrapesJsMessageTemplateEditor>(ServiceLifetime.Singleton);
    }

    public int Order => 1;

    #endregion IDependencyRegistrar Members
}