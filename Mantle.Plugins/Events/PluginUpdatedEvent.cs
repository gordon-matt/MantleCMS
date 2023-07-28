namespace Mantle.Plugins.Events;

/// <summary>
/// Represents the plugin updated event
/// </summary>
public class PluginUpdatedEvent
{
    #region Ctor

    /// <summary>
    /// Ctor
    /// </summary>
    /// <param name="plugin">Updated plugin</param>
    public PluginUpdatedEvent(PluginDescriptor plugin)
    {
        this.Plugin = plugin;
    }

    #endregion Ctor

    #region Properties

    /// <summary>
    /// Updated plugin
    /// </summary>
    public PluginDescriptor Plugin { get; private set; }

    #endregion Properties
}