using Autofac;
using Mantle.Infrastructure;
using Mantle.Web.CommonResources.ScriptBuilder.Toasts;
using Mantle.Web.Infrastructure;
using Microsoft.Extensions.Configuration;

namespace Mantle.Web.CommonResources.Infrastructure;

public class DependencyRegistrar : IDependencyRegistrar
{
    #region IDependencyRegistrar Members

    public void Register(ContainerBuilder builder, ITypeFinder typeFinder, IConfiguration configuration)
    {
        var options = new MantleCommonResourceOptions();
        configuration.GetSection("MantleCommonResourceOptions").Bind(options);

        if (options.ScriptBuilderDefaults.UseDefaultToastProvider)
        {
            builder.RegisterType<NotifyJsToastBuilder>().As<IToastsScriptBuilder>().SingleInstance();
        }

        builder.RegisterType<RequireJSConfigProvider>().As<IRequireJSConfigProvider>().SingleInstance();
    }

    public int Order => 1;

    #endregion IDependencyRegistrar Members
}