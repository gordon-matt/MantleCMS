using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Mantle.Helpers;
using Mantle.Infrastructure.Configuration;
using Mantle.Plugins;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Mantle.Infrastructure
{
    public class AutofacEngine : IEngine, IDisposable
    {
        #region Private Members

        private AutofacContainerManager containerManager;
        private bool disposed = false;

        #endregion Private Members

        #region Properties

        /// <summary>
        /// Gets or sets service provider
        /// </summary>
        public virtual IServiceProvider ServiceProvider { get; private set; }

        #endregion Properties

        #region IEngine Members

        /// <summary>
        /// Add and configure services
        /// </summary>
        /// <param name="services">Collection of service descriptors</param>
        /// <param name="configuration">Configuration root of the application</param>
        /// <returns>Service provider</returns>
        public virtual IServiceProvider ConfigureServices(ContainerBuilder containerBuilder, IConfigurationRoot configuration)
        {
            //find startup configurations provided by other assemblies
            var typeFinder = new WebAppTypeFinder();
            var startupConfigurations = typeFinder.FindClassesOfType<IMantleStartup>();

            //create and sort instances of startup configurations
            var instances = startupConfigurations
                .Where(startup => PluginManager.FindPlugin(startup)?.Installed ?? true) //ignore not installed plugins
                .Select(startup => (IMantleStartup)Activator.CreateInstance(startup))
                .OrderBy(startup => startup.Order);

            //configure services
            foreach (var instance in instances)
            {
                instance.ConfigureServices(containerBuilder, configuration);
            }

            //register dependencies
            RegisterDependencies(containerBuilder, typeFinder);

            var options = ServiceProvider.GetService<MantleInfrastructureOptions>();
            //run startup tasks
            if (!options.IgnoreStartupTasks)
            {
                RunStartupTasks(typeFinder);
            }

            //resolve assemblies here. otherwise, plugins can throw an exception when rendering views
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;

            //set App_Data path as base data directory (required to create and save SQL Server Compact database file in App_Data folder)
            AppDomain.CurrentDomain.SetData("DataDirectory", CommonHelper.MapPath("~/App_Data/"));

            return ServiceProvider;
        }

        /// <summary>
        /// Configure HTTP request pipeline
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        public virtual void ConfigureRequestPipeline(IApplicationBuilder application)
        {
            //find startup configurations provided by other assemblies
            var typeFinder = Resolve<ITypeFinder>();
            var startupConfigurations = typeFinder.FindClassesOfType<IMantleStartup>();

            //create and sort instances of startup configurations
            var instances = startupConfigurations
                .Where(startup => PluginManager.FindPlugin(startup)?.Installed ?? true) //ignore not installed plugins
                .Select(startup => (IMantleStartup)Activator.CreateInstance(startup))
                .OrderBy(startup => startup.Order);

            //configure request pipeline
            foreach (var instance in instances)
            {
                instance.Configure(application);
            }
        }

        public virtual T Resolve<T>() where T : class
        {
            return containerManager.Resolve<T>();
            //return (T)GetServiceProvider().GetRequiredService(typeof(T));
        }

        public T Resolve<T>(IDictionary<string, object> ctorArgs) where T : class
        {
            return containerManager.Resolve<T>(ctorArgs);
        }

        public virtual object Resolve(Type type)
        {
            return containerManager.Resolve(type);
            //return GetServiceProvider().GetRequiredService(type);
        }

        public T ResolveNamed<T>(string name) where T : class
        {
            return containerManager.ResolveNamed<T>(name);
        }

        public virtual IEnumerable<T> ResolveAll<T>()
        {
            return containerManager.ResolveAll<T>();
            //return (IEnumerable<T>)GetServiceProvider().GetServices(typeof(T));
        }

        public IEnumerable<T> ResolveAllNamed<T>(string name)
        {
            return containerManager.ResolveAllNamed<T>(name);
        }

        public virtual object ResolveUnregistered(Type type)
        {
            return containerManager.ResolveUnregistered(type);
        }

        public bool TryResolve<T>(out T instance) where T : class
        {
            return containerManager.TryResolve<T>(out instance);
        }

        public bool TryResolve(Type serviceType, out object instance)
        {
            return containerManager.TryResolve(serviceType, out instance);
        }

        #endregion IEngine Members

        #region Non-Public Methods

        /// <summary>
        /// Get IServiceProvider
        /// </summary>
        /// <returns>IServiceProvider</returns>
        protected virtual IServiceProvider GetServiceProvider()
        {
            var accessor = ServiceProvider.GetService<IHttpContextAccessor>();
            var context = accessor.HttpContext;
            return context != null ? context.RequestServices : ServiceProvider;
        }

        /// <summary>
        /// Register dependencies using Autofac
        /// </summary>
        /// <param name="containerBuilder">Container Builder</param>
        /// <param name="typeFinder">Type finder</param>
        protected virtual IServiceProvider RegisterDependencies(ContainerBuilder containerBuilder, ITypeFinder typeFinder)
        {
            //register engine
            containerBuilder.RegisterInstance(this).As<IEngine>().SingleInstance();

            //register type finder
            containerBuilder.RegisterInstance(typeFinder).As<ITypeFinder>().SingleInstance();

            //find dependency registrars provided by other assemblies
            var dependencyRegistrars = typeFinder.FindClassesOfType<IDependencyRegistrar>();

            //create and sort instances of dependency registrars
            var instances = dependencyRegistrars
                //.Where(dependencyRegistrar => PluginManager.FindPlugin(dependencyRegistrar).Return(plugin => plugin.Installed, true)) //ignore not installed plugins
                .Select(dependencyRegistrar => (IDependencyRegistrar)Activator.CreateInstance(dependencyRegistrar))
                .OrderBy(dependencyRegistrar => dependencyRegistrar.Order);

            //register all provided dependencies
            foreach (var dependencyRegistrar in instances)
            {
                dependencyRegistrar.Register(containerBuilder, typeFinder);
            }

            ////populate Autofac container builder with the set of registered service descriptors
            //containerBuilder.Populate(services);

            //create service provider
            var container = containerBuilder.Build();
            ServiceProvider = new AutofacServiceProvider(container);
            containerManager = new AutofacContainerManager(container);
            return ServiceProvider;
        }

        /// <summary>
        /// Run startup tasks
        /// </summary>
        /// <param name="typeFinder">Type finder</param>
        protected virtual void RunStartupTasks(ITypeFinder typeFinder)
        {
            //find startup tasks provided by other assemblies
            var startupTasks = typeFinder.FindClassesOfType<IStartupTask>();

            //create and sort instances of startup tasks
            //we startup this interface even for not installed plugins.
            //otherwise, DbContext initializers won't run and a plugin installation won't work
            var instances = startupTasks
                .Select(startupTask => (IStartupTask)Activator.CreateInstance(startupTask))
                .OrderBy(startupTask => startupTask.Order);

            //execute tasks
            foreach (var task in instances)
                task.Execute();
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            //check for assembly already loaded
            var assembly = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(a => a.FullName == args.Name);
            if (assembly != null)
                return assembly;

            //get assembly from TypeFinder
            var tf = Resolve<ITypeFinder>();
            assembly = tf.GetAssemblies().FirstOrDefault(a => a.FullName == args.Name);
            return assembly;
        }

        #endregion Non-Public Methods

        #region IDisposable Members

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposed)
            {
                return;
            }

            if (disposing)
            {
                containerManager.Dispose();
                // Free any other managed objects here.
            }

            // Free any unmanaged objects here.
            disposed = true;
        }

        #endregion IDisposable Members
    }
}