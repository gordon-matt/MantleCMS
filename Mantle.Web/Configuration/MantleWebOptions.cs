namespace Mantle.Web.Configuration
{
    public class MantleWebOptions
    {
        public MantleWebOptions()
        {
            Resources = new Resources();
        }

        /// <summary>
        /// Gets or sets a value indicating whether to clear /Plugins/bin directory on application startup
        /// </summary>
        public bool ClearPluginShadowDirectoryOnStartup { get; set; }

        public Resources Resources { get; set; }
    }

    public class Resources
    {
        public string ScriptsBasePath { get; set; }

        public string StylesBasePath { get; set; }
    }
}