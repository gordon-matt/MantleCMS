﻿using System.Collections.Generic;

namespace Mantle.Plugins.Events
{
    /// <summary>
    /// Plugins uploaded event
    /// </summary>
    public class PluginsUploadedEvent
    {
        #region Ctor

        /// <summary>
        /// Ctor
        /// </summary>
        /// <param name="uploadedPlugins">Uploaded plugins</param>
        public PluginsUploadedEvent(IList<PluginDescriptor> uploadedPlugins)
        {
            this.UploadedPlugins = uploadedPlugins;
        }

        #endregion Ctor

        #region Properties

        /// <summary>
        /// Uploaded plugins
        /// </summary>
        public IList<PluginDescriptor> UploadedPlugins { get; private set; }

        #endregion Properties
    }
}