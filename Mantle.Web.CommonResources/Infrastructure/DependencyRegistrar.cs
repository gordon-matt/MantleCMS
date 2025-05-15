using Dependo;
using Mantle.Web.CommonResources.ScriptBuilder.Toasts;
using Mantle.Web.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mantle.Web.CommonResources.Infrastructure;

public class DependencyRegistrar : IDependencyRegistrar
{
    #region IDependencyRegistrar Members

    public void Register(IContainerBuilder builder, ITypeFinder typeFinder, IConfiguration configuration)
    {
        var options = new MantleCommonResourceOptions();
        configuration.GetSection("MantleCommonResourceOptions").Bind(options);

        if (options.ScriptBuilderDefaults.UseDefaultToastProvider)
        {
            builder.Register<IToastsScriptBuilder, NotifyJsToastBuilder>(ServiceLifetime.Singleton);
        }

        builder.Register<IRequireJSConfigProvider, RequireJSConfigProvider>(ServiceLifetime.Singleton);
    }

    public int Order => 1;

    #endregion IDependencyRegistrar Members
}