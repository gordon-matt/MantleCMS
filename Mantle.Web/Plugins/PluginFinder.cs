using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Mantle.Web.Plugins
{
    /// <summary>
    /// Plugin finder
    /// </summary>
    public class PluginFinder : IPluginFinder
    {
        #region Fields

        private IList<PluginDescriptor> _plugins;

        private bool _arePluginsLoaded = false;

        #endregion Fields

        #region Utilities

        /// <summary>
        /// Ensure plugins are loaded
        /// </summary>
        protected virtual void EnsurePluginsAreLoaded()
        {
            if (!_arePluginsLoaded)
            {
                var foundPlugins = PluginManager.ReferencedPlugins.ToList();
                foundPlugins.Sort(); //sort
                _plugins = foundPlugins.ToList();

                _arePluginsLoaded = true;
            }
        }

        #endregion Utilities

        #region Methods

        /// <summary>
        /// Check whether the plugin is available for a specific tenant
        /// </summary>
        /// <param name="pluginDescriptor">Plugin descriptor to check</param>
        /// <param name="tenantId">Tenant identifier to check</param>
        /// <returns>true - available; false - no</returns>
        public virtual bool AuthenticateTenant(PluginDescriptor pluginDescriptor, int? tenantId)
        {
            if (pluginDescriptor == null)
            {
                throw new ArgumentNullException("pluginDescriptor");
            }

            //no validation required
            if (!tenantId.HasValue)
            {
                return true;
            }

            if (pluginDescriptor.LimitedToTenants.Count == 0)
            {
                return true;
            }

            return pluginDescriptor.LimitedToTenants.Contains(tenantId.Value);
        }

        /// <summary>
        /// Gets plugins
        /// </summary>
        /// <typeparam name="T">The type of plugins to get.</typeparam>
        /// <param name="installedOnly">A value indicating whether to load only installed plugins</param>
        /// <param name="tenantId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <returns>Plugins</returns>
        public virtual IEnumerable<T> GetPlugins<T>(bool installedOnly = true, int? tenantId = null) where T : class, IPlugin
        {
            EnsurePluginsAreLoaded();

            foreach (var plugin in _plugins)
            {
                if (typeof(T).GetTypeInfo().IsAssignableFrom(plugin.PluginType))
                {
                    if (!installedOnly || plugin.Installed)
                    {
                        if (AuthenticateTenant(plugin, tenantId))
                        {
                            yield return plugin.Instance<T>();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get plugin descriptors
        /// </summary>
        /// <param name="installedOnly">A value indicating whether to load only installed plugins</param>
        /// <param name="tenantId">Load records allowed only in a specified store; pass 0 to load all records</param>
        /// <returns>Plugin descriptors</returns>
        public virtual IEnumerable<PluginDescriptor> GetPluginDescriptors(bool installedOnly = true, int? tenantId = null)
        {
            EnsurePluginsAreLoaded();

            foreach (var plugin in _plugins)
            {
                if (!installedOnly || plugin.Installed)
                {
                    if (AuthenticateTenant(plugin, tenantId))
                    {
                        yield return plugin;
                    }
                }
            }
        }

        /// <summary>
        /// Get plugin descriptors
        /// </summary>
        /// <typeparam name="T">The type of plugin to get.</typeparam>
        /// <param name="installedOnly">A value indicating whether to load only installed plugins</param>
        /// <param name="tenantId">Load records allowed only in a specified tenant; pass 0 to load all records</param>
        /// <returns>Plugin descriptors</returns>
        public virtual IEnumerable<PluginDescriptor> GetPluginDescriptors<T>(bool installedOnly = true, int? tenantId = null)
            where T : class, IPlugin
        {
            EnsurePluginsAreLoaded();

            foreach (var plugin in _plugins)
            {
                if (typeof(T).IsAssignableFrom(plugin.PluginType))
                {
                    if (!installedOnly || plugin.Installed)
                    {
                        if (AuthenticateTenant(plugin, tenantId))
                        {
                            yield return plugin;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Get a plugin descriptor by its system name
        /// </summary>
        /// <param name="systemName">Plugin system name</param>
        /// <param name="installedOnly">A value indicating whether to load only installed plugins</param>
        /// <returns>>Plugin descriptor</returns>
        public virtual PluginDescriptor GetPluginDescriptorBySystemName(string systemName, bool installedOnly = true)
        {
            return GetPluginDescriptors(installedOnly)
                .SingleOrDefault(p => p.SystemName.Equals(systemName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Get a plugin descriptor by its system name
        /// </summary>
        /// <typeparam name="T">The type of plugin to get.</typeparam>
        /// <param name="systemName">Plugin system name</param>
        /// <param name="installedOnly">A value indicating whether to load only installed plugins</param>
        /// <returns>>Plugin descriptor</returns>
        public virtual PluginDescriptor GetPluginDescriptorBySystemName<T>(string systemName, bool installedOnly = true) where T : class, IPlugin
        {
            return GetPluginDescriptors<T>(installedOnly)
                .SingleOrDefault(p => p.SystemName.Equals(systemName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Reload plugins
        /// </summary>
        public virtual void ReloadPlugins()
        {
            _arePluginsLoaded = false;
            EnsurePluginsAreLoaded();
        }

        #endregion Methods
    }
}