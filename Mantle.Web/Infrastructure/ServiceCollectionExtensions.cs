using Mantle.Infrastructure.Configuration;
using Mantle.Plugins.Configuration;
using Mantle.Tasks.Configuration;
using Mantle.Web.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mantle.Web.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static void ConfigureMantleOptions(this IServiceCollection services, IConfigurationRoot configuration)
        {
            var infrastructureOptions = new MantleInfrastructureOptions();
            configuration.Bind(infrastructureOptions);
            services.AddSingleton(infrastructureOptions);

            var pluginOptions = new MantlePluginOptions();
            configuration.Bind(pluginOptions);
            services.AddSingleton(pluginOptions);

            var tasksOptions = new MantleTasksOptions();
            configuration.Bind(tasksOptions);
            services.AddSingleton(tasksOptions);

            var webOptions = new MantleWebOptions();
            configuration.Bind(webOptions);
            services.AddSingleton(webOptions);
        }
    }
}