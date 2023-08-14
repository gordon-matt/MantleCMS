using Autofac;
using Mantle.Infrastructure;
using Mantle.Web.CommonResources.ScriptBuilder.Toasts;
using Mantle.Web.Infrastructure;

namespace Mantle.Web.CommonResources.Infrastructure;

public class DependencyRegistrar : IDependencyRegistrar
{
    #region IDependencyRegistrar Members

    public void Register(ContainerBuilder builder, ITypeFinder typeFinder)
    {
        builder.RegisterType<NotifyJsToastBuilder>().As<IToastsScriptBuilder>().SingleInstance();
        builder.RegisterType<RequireJSConfigProvider>().As<IRequireJSConfigProvider>().SingleInstance();
    }

    public int Order => 1;

    #endregion IDependencyRegistrar Members
}