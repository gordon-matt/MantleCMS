﻿using System;
using System.Collections.Generic;
using Autofac;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;

namespace Mantle.Infrastructure
{
    /// <summary>
    /// Classes implementing this interface can serve as a portal for the various services composing the Mantle engine.
    /// Edit functionality, modules and implementations access most Mantle functionality through this interface.
    /// </summary>
    public interface IEngine
    {
        /// <summary>
        /// Add and configure services
        /// </summary>
        /// <param name="containerBuilder">Container Builder</param>
        /// <param name="configuration">Configuration root of the application</param>
        /// <returns>Service provider</returns>
        IServiceProvider ConfigureServices(ContainerBuilder containerBuilder, IConfigurationRoot configuration);

        /// <summary>
        /// Configure HTTP request pipeline
        /// </summary>
        /// <param name="application">Builder for configuring an application's request pipeline</param>
        void ConfigureRequestPipeline(IApplicationBuilder application);

        T Resolve<T>() where T : class;

        T Resolve<T>(IDictionary<string, object> ctorArgs) where T : class;

        object Resolve(Type type);

        T ResolveNamed<T>(string name) where T : class;

        IEnumerable<T> ResolveAll<T>();

        IEnumerable<T> ResolveAllNamed<T>(string name);

        object ResolveUnregistered(Type type);

        bool TryResolve<T>(out T instance) where T : class;

        bool TryResolve(Type serviceType, out object instance);
    }
}