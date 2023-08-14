using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mantle.Web.CommonResources.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static void ConfigureMantleCommonResourceOptions(this IServiceCollection services, IConfigurationRoot configuration)
    {
        var options = new MantleCommonResourceOptions();
        configuration.Bind(options);
        services.AddSingleton(options);
    }
}