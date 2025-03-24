using Autofac;
using Mantle.Logging.Services;
using Microsoft.Extensions.Configuration;

namespace Mantle.Logging.Infrastructure;

public class DependencyRegistrar : IDependencyRegistrar
{
    #region IDependencyRegistrar Members

    public void Register(ContainerBuilder builder, ITypeFinder typeFinder, IConfiguration configuration) =>
        builder.RegisterType<LogService>().As<ILogService>().InstancePerDependency();

    public int Order => 0;

    #endregion IDependencyRegistrar Members
}