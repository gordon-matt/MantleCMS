using Autofac;
using Autofac.Extensions.DependencyInjection;
using Mantle.Helpers;
using Mantle.Infrastructure.Configuration;
using Mantle.Plugins;
using Mantle.Plugins.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Reflection;

namespace Mantle.Infrastructure
{
    public class MantleAutofacServiceProviderFactory : IServiceProviderFactory<ContainerBuilder>
    {
        private readonly Action<ContainerBuilder> configurationAction;
        private IServiceCollection services;

        /// <summary>
        /// Initializes a new instance of the <see cref="AutofacServiceProviderFactory"/> class.
        /// </summary>
        /// <param name="configurationAction">Action on a <see cref="ContainerBuilder"/> that adds component registrations to the conatiner.</param>
        public MantleAutofacServiceProviderFactory(Action<ContainerBuilder> configurationAction = null)
        {
            this.configurationAction = configurationAction ?? (builder => { });
        }

        /// <summary>
        /// Creates a container builder from an <see cref="IServiceCollection" />.
        /// </summary>
        /// <param name="services">The collection of services.</param>
        /// <returns>A container builder that can be used to create an <see cref="IServiceProvider" />.</returns>
        public ContainerBuilder CreateBuilder(IServiceCollection services)
        {
            this.services = services; // To use in CreateServiceProvider()

            var builder = new ContainerBuilder();
            builder.Populate(services);
            configurationAction(builder);
            return builder;
        }

        /// <summary>
        /// Creates an <see cref="IServiceProvider" /> from the container builder.
        /// </summary>
        /// <param name="containerBuilder">The container builder.</param>
        /// <returns>An <see cref="IServiceProvider" />.</returns>
        public IServiceProvider CreateServiceProvider(ContainerBuilder containerBuilder)
        {
            if (containerBuilder == null)
            {
                throw new ArgumentNullException(nameof(containerBuilder));
            }


            //most of API providers require TLS 1.2 nowadays
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            //set base application path
            var provider = services.BuildServiceProvider();
            var hostingEnvironment = provider.GetRequiredService<IHostingEnvironment>();
            var options = provider.GetRequiredService<MantlePluginOptions>();
            CommonHelper.BaseDirectory = hostingEnvironment.ContentRootPath;

            var partManager = provider.GetService<ApplicationPartManager>();

            //initialize plugins
            PluginManager.Initialize(partManager, hostingEnvironment, options);

            var configuration = provider.GetService<IConfigurationRoot>();

            var engine = new AutofacEngine();
            var serviceProvider = engine.ConfigureServices(containerBuilder, configuration);
            EngineContext.Create(engine);

            var options1 = provider.GetService<MantleInfrastructureOptions>();
            //run startup tasks
            if (!options1.IgnoreStartupTasks)
            {
                engine.RunStartupTasks();
            }

            //// TODO: This should not be here. Find somewhere else to put it.. problem is making sure it's after engine context has been initialized above.
            ////if (DataSettingsHelper.IsDatabaseInstalled && FrameworkConfigurationSection.Instance.ScheduledTasks.Enabled)
            ////{
            //TaskManager.Instance.Initialize();
            //TaskManager.Instance.Start();
            ////}

            return serviceProvider; // AutofacServiceProvider
        }
    }
}