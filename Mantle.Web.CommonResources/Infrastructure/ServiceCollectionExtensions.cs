using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mantle.Web.CommonResources.Infrastructure;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection ConfigureMantleCommonResourceOptions(IConfigurationRoot configuration) =>
            services.Configure<MantleCommonResourceOptions>(configuration.GetSection("MantleCommonResourceOptions"));
    }
}