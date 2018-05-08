using System;
using Mantle.Configuration;
using Mantle.Infrastructure;
using Mantle.Tasks;
using Mantle.Web.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mantle.Web.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceProvider ConfigureMantleServices(this IServiceCollection services, IConfigurationRoot configuration)
        {
            var mantleOptions = new MantleOptions();
            configuration.Bind(mantleOptions);
            services.AddSingleton(mantleOptions);

            var webOptions = new MantleWebOptions();
            configuration.Bind(webOptions);
            services.AddSingleton(webOptions);

            //// Tell Mantle it is a website (not something like unit test or whatever)
            //Mantle.Hosting.HostingEnvironment.IsHosted = true;

            var engine = EngineContext.Create(new AutofacEngine());
            engine.Initialize(services);
            var serviceProvider = engine.ConfigureServices(services, configuration);

            //if (DataSettingsHelper.DatabaseIsInstalled)
            //{
            TaskManager.Instance.Initialize();
            TaskManager.Instance.Start();
            //}

            return serviceProvider;
        }
    }
}