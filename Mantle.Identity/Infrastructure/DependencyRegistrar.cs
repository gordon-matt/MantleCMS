using Autofac;
using Mantle.Localization;
using Microsoft.Extensions.Configuration;

namespace Mantle.Identity.Infrastructure;

public class DependencyRegistrar : IDependencyRegistrar
{
    #region IDependencyRegistrar Members

    public void Register(ContainerBuilder builder, ITypeFinder typeFinder, IConfiguration configuration)
    {
        // localization
        builder.RegisterType<LanguagePackInvariant>().As<ILanguagePack>().SingleInstance();
    }

    public int Order
    {
        get { return 0; }
    }

    #endregion IDependencyRegistrar Members
}