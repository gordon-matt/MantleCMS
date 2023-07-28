namespace Mantle.Web.Mvc.Resources;

public enum ResourceLocation : byte
{
    /// <summary>
    /// Pinned to the declaring place but moves to the bundle if enabled.
    /// </summary>
    Auto,

    /// <summary>
    /// Pinned to the declaring place
    /// </summary>
    None,

    Head,

    Foot,
}