using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Mantle.Infrastructure
{
    public class WebAppTypeFinder : AppDomainTypeFinder
    {
        private bool binFolderAssembliesLoaded = false;

        public WebAppTypeFinder() : base()
        {
            EnsureBinFolderAssembliesLoaded = true; //TODO: put into web.config
            //this._ensureBinFolderAssembliesLoaded = MantleConfigurationSection.Instance.DynamicDiscovery;
        }

        /// <summary>
        /// <para>Gets or sets wether assemblies in the bin folder of the web application should be specificly checked</para>
        /// <para>for beeing loaded on application load. This is need in situations where plugins need to be loaded in</para>
        /// <para>the AppDomain after the application been reloaded.</para>
        /// </summary>
        public bool EnsureBinFolderAssembliesLoaded { get; set; }

        /// <summary>
        /// Gets a physical disk path of \Bin directory
        /// </summary>
        /// <returns>The physical path. E.g. "c:\inetpub\wwwroot\bin"</returns>
        public virtual string GetBinDirectory()
        {
            string location = Assembly.GetEntryAssembly().Location;
            return Path.GetDirectoryName(location);
        }

        public override IEnumerable<Assembly> GetAssemblies()
        {
            if (EnsureBinFolderAssembliesLoaded && !binFolderAssembliesLoaded)
            {
                binFolderAssembliesLoaded = true;
                string binPath = GetBinDirectory();
                LoadMatchingAssemblies(binPath);
            }

            return base.GetAssemblies();
        }
    }
}