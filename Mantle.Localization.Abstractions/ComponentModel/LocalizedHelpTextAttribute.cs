namespace Mantle.Localization.ComponentModel;

[AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
public class LocalizedHelpTextAttribute : Attribute
{
    private static IStringLocalizer localizer;

    private static IStringLocalizer T
    {
        get
        {
            localizer ??= EngineContext.Current.Resolve<IStringLocalizer>();
            return localizer;
        }
    }

    public LocalizedHelpTextAttribute(string resourceKey)
    {
        ResourceKey = resourceKey;
    }

    public string ResourceKey { get; set; }

    public string HelpText => T[ResourceKey];
}