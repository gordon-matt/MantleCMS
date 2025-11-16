namespace Mantle.Plugins;

/// <summary>
/// Plugin extensions
/// </summary>
public static class PluginExtensions
{
    private static readonly List<string> SupportedLogoImageExtensions = ["jpg", "png", "gif"];

    extension(PluginDescriptor pluginDescriptor)
    {
        /// <summary>
        /// Get logo URL
        /// </summary>
        /// <returns>Logo URL</returns>
        public string GetLogoUrl()
        {
            ArgumentNullException.ThrowIfNull(pluginDescriptor);

            if (pluginDescriptor.OriginalAssemblyFile?.Directory is null)
            {
                return null;
            }

            var pluginDirectory = pluginDescriptor.OriginalAssemblyFile.Directory;

            string logoExtension = SupportedLogoImageExtensions.FirstOrDefault(ext => File.Exists(Path.Combine(pluginDirectory.FullName, "logo." + ext)));

            if (string.IsNullOrWhiteSpace(logoExtension))
            {
                return null; //No logo file was found with any of the supported extensions.
            }

            return $"{CommonHelper.BaseDirectory}plugins/{pluginDirectory.Name}/logo.{logoExtension}";
        }
    }
}