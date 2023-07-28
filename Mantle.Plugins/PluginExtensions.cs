namespace Mantle.Plugins;

/// <summary>
/// Plugin extensions
/// </summary>
public static class PluginExtensions
{
    private static readonly List<string> SupportedLogoImageExtensions = new()
    {
        "jpg",
        "png",
        "gif"
    };

    /// <summary>
    /// Get logo URL
    /// </summary>
    /// <param name="pluginDescriptor">Plugin descriptor</param>
    /// <param name="webHelper">Web helper</param>
    /// <returns>Logo URL</returns>
    public static string GetLogoUrl(this PluginDescriptor pluginDescriptor)
    {
        if (pluginDescriptor == null)
        {
            throw new ArgumentNullException(nameof(pluginDescriptor));
        }

        if (pluginDescriptor.OriginalAssemblyFile == null || pluginDescriptor.OriginalAssemblyFile.Directory == null)
        {
            return null;
        }

        var pluginDirectory = pluginDescriptor.OriginalAssemblyFile.Directory;

        string logoExtension = SupportedLogoImageExtensions.FirstOrDefault(ext => File.Exists(Path.Combine(pluginDirectory.FullName, "logo." + ext)));

        if (string.IsNullOrWhiteSpace(logoExtension))
        {
            return null; //No logo file was found with any of the supported extensions.
        }

        string logoUrl = $"{CommonHelper.BaseDirectory}plugins/{pluginDirectory.Name}/logo.{logoExtension}";
        return logoUrl;
    }
}