using System.ComponentModel;

namespace Mantle.Localization.ComponentModel;

//TODO: Implement this with a custom DataAnnotationsModelMetadataProvider?
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
public class LocalizedDisplayNameAttribute : DisplayNameAttribute
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

    public LocalizedDisplayNameAttribute(string resourceKey)
        : base(resourceKey)
    {
        ResourceKey = resourceKey;
    }

    public string ResourceKey { get; set; }

    public override string DisplayName => T[ResourceKey];
}