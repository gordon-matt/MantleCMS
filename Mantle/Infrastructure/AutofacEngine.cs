using System;
using System.Collections.Generic;
using System.Linq;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Mantle.Collections;
using Mantle.Exceptions;
using Mantle.Infrastructure.DependencyManagement;
using Microsoft.Extensions.DependencyInjection;

namespace Mantle.Infrastructure
{
    public class AutofacEngine : IEngine
    {
        #region Fields

        private AutofacContainerManager containerManager;
        private bool disposed = false;

        #endregion Fields

        #region Ctor

        /// <summary>
        /// Creates an instance of the content engine using default settings and configuration.
        /// </summary>
        public AutofacEngine(IServiceCollection services)
            : this(services, new ContainerConfigurer())
        {
        }

        public AutofacEngine(IServiceCollection services, ContainerConfigurer configurer)
        {
            InitializeContainer(services, configurer);
        }

        #endregion Ctor

        #region Utilities

        private void RunStartupTasks()
        {
            //if (!DataSettingsHelper.IsDatabaseInstalled)
            //{
            //    return;
            //}

            var typeFinder = EngineContext.Current.Resolve<ITypeFinder>();
            var startUpTaskTypes = typeFinder.FindClassesOfType<IStartupTask>();
            var startUpTasks = new List<IStartupTask>();
            foreach (var startUpTaskType in startUpTaskTypes)
            {
                startUpTasks.Add((IStartupTask)Activator.CreateInstance(startUpTaskType));
            }
            //sort
            startUpTasks = startUpTasks.AsQueryable().OrderBy(st => st.Order).ToList();
            foreach (var startUpTask in startUpTasks)
            {
                startUpTask.Execute();
            }
        }

        private void InitializeContainer(IServiceCollection services, ContainerConfigurer configurer)
        {
            var builder = new ContainerBuilder();

            if (!services.IsNullOrEmpty())
            {
                builder.Populate(services);
            }

            var container = builder.Build();

            containerManager = new AutofacContainerManager(container);
            configurer.Configure(this, containerManager);

            //_containerManager.NotifyCompleted(container);
        }

        #endregion Utilities

        #region Methods

        /// <summary>
        /// Initialize components and plugins in the Kore environment.
        /// </summary>
        /// <param name="config">Config</param>
        public void Initialize()
        {
            ////startup tasks
            //if (!KoreConfigurationSection.Instance.IgnoreStartupTasks)
            //{
            RunStartupTasks();
            //}
        }

        public T Resolve<T>() where T : class
        {
            return ContainerManager.Resolve<T>();
        }

        public T Resolve<T>(IDictionary<string, object> ctorArgs) where T : class
        {
            return ContainerManager.Resolve<T>(ctorArgs);
        }

        public T ResolveNamed<T>(string name) where T : class
        {
            return ContainerManager.ResolveNamed<T>(name);
        }

        public object Resolve(Type type)
        {
            return ContainerManager.Resolve(type);
        }

        public IEnumerable<T> ResolveAll<T>()
        {
            return ContainerManager.ResolveAll<T>();
        }

        public IEnumerable<T> ResolveAllNamed<T>(string name)
        {
            return ContainerManager.ResolveAllNamed<T>(name);
        }

        public virtual object ResolveUnregistered(Type type)
        {
            foreach (var constructor in type.GetConstructors())
            {
                try
                {
                    //try to resolve constructor parameters
                    var parameters = constructor.GetParameters().Select(parameter =>
                    {
                        var service = Resolve(parameter.ParameterType);
                        if (service == null)
                        {
                            throw new MantleException("Unknown dependency");
                        }
                        return service;
                    });

                    //all is ok, so create instance
                    return Activator.CreateInstance(type, parameters.ToArray());
                }
                catch (MantleException) { }
            }
            throw new MantleException("No constructor was found that had all the dependencies satisfied.");
        }

        public bool TryResolve<T>(out T instance)
        {
            return ContainerManager.TryResolve<T>(out instance);
        }

        public bool TryResolve(Type serviceType, out object instance)
        {
            return ContainerManager.TryResolve(serviceType, out instance);
        }

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
                containerManager.Container.Dispose();
                // Free any other managed objects here.
            }

            // Free any unmanaged objects here.
            disposed = true;
        }

        #endregion Methods

        #region Properties

        public IContainerManager ContainerManager
        {
            get { return containerManager; }
        }

        public IServiceProvider ServiceProvider
        {
            get { return new AutofacServiceProvider(containerManager.Container); }
        }

        #endregion Properties
    }
}