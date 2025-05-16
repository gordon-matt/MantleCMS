using Microsoft.Extensions.DependencyInjection;

namespace Mantle.Web.Infrastructure;

public static class MvcBuilderExtensions
{
    public static IMvcBuilder AddMantleEmbeddedFileProviders(this IMvcBuilder builder)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies()
            .Where(a => a.ExportedTypes.Any(t => t.GetInterfaces().Contains(typeof(IEmbeddedFileProviderRegistrar))));

        // View Components won't work unless we do this.
        foreach (var assembly in assemblies)
        {
            builder.AddApplicationPart(assembly);
        }

        return builder;
    }
}