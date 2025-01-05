namespace Mantle.Localization;

public static class CacheKeys
{
    /// <summary>
    /// {0}: Tenant ID, {1}: Culture Code
    /// </summary>
    public const string LocalizableStringsFormat = "Mantle.Localization.CacheKeys.LocalizableStrings_{0}_{1}";

    /// <summary>
    /// {0}: Tenant ID
    /// </summary>
    public const string LocalizableStringsPatternFormat = "Mantle.Localization.CacheKeys.LocalizableStrings_{0}_.*";
}