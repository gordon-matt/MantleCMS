using System;
using System.Collections.Generic;
using System.Linq;
using Mantle.Infrastructure;
using Mantle.Web.Plugins;
using Microsoft.AspNetCore.Routing;

namespace Mantle.Web.Mvc.Routing
{
    public interface IRoutePublisher
    {
        void RegisterRoutes(IRouteBuilder routeCollection);
    }

    public class RoutePublisher : IRoutePublisher
    {
        protected readonly ITypeFinder typeFinder;

        public RoutePublisher(ITypeFinder typeFinder)
        {
            this.typeFinder = typeFinder;
        }

        protected virtual PluginDescriptor FindPlugin(Type providerType)
        {
            if (providerType == null)
            {
                throw new ArgumentNullException("providerType");
            }

            foreach (var plugin in PluginManager.ReferencedPlugins)
            {
                if (plugin.ReferencedAssembly == null)
                {
                    continue;
                }

                if (plugin.ReferencedAssembly.FullName == providerType.Assembly.FullName)
                {
                    return plugin;
                }
            }

            return null;
        }

        public virtual void RegisterRoutes(IRouteBuilder routes)
        {
            var routeProviderTypes = typeFinder.FindClassesOfType<IRouteProvider>();
            var routeProviders = new List<IRouteProvider>();

            foreach (var providerType in routeProviderTypes)
            {
                //Ignore not installed plugins
                var plugin = FindPlugin(providerType);
                if (plugin != null && !plugin.Installed)
                {
                    continue;
                }

                var provider = Activator.CreateInstance(providerType) as IRouteProvider;
                routeProviders.Add(provider);
            }

            routeProviders = routeProviders.OrderByDescending(rp => rp.Priority).ToList();
            routeProviders.ForEach(rp => rp.RegisterRoutes(routes));
        }
    }
}