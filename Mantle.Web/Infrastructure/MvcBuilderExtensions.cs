using Dependo;
using Microsoft.Extensions.DependencyInjection;

namespace Mantle.Web.Infrastructure;

public static class MvcBuilderExtensions
{
    extension(IMvcBuilder builder)
    {
        public IMvcBuilder AddMantleEmbeddedFileProviders()
        {
            // The registrar list comes from the Dependo catalog when the host was compiled with it.
            // Plugin assemblies are added later by PluginManager.
            var typeFinder = new WebAppTypeFinder();
            var assemblies = typeFinder.FindClassesOfType<IEmbeddedFileProviderRegistrar>()
                .Select(type => type.Assembly)
                .Distinct();

            foreach (var assembly in assemblies)
            {
                builder.AddApplicationPart(assembly);
            }

            return builder;
        }
    }
}