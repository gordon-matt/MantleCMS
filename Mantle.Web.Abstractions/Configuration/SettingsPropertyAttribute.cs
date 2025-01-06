namespace Mantle.Web.Configuration;

[AttributeUsage(AttributeTargets.Property)]
public class SettingsPropertyAttribute : Attribute
{
    public static readonly SettingsPropertyAttribute Default = new();

    public SettingsPropertyAttribute() : this(default)
    {
    }

    public SettingsPropertyAttribute(object defaultValue)
    {
        DefaultValue = defaultValue;
    }

    public object DefaultValue { get; set; }

    /// <summary>
    /// Optionally use custom declaration. Example: `viewModel.myValue = ko.observable();`
    /// </summary>
    public string Declaration { get; set; }

    /// <summary>
    /// Optionally use custom value assignment. Example: `viewModel.myValue(data.MyValue);`
    /// </summary>
    public string Assignment { get; set; }

    /// <summary>
    /// Optionally use custom deletion. Example: `delete viewModel.myValue;`
    /// </summary>
    public string CleanUp { get; set; }

    /// <summary>
    /// Optionally use custom value assignment. Example: `MyValue: viewModel.myValue()`
    /// </summary>
    public string Save { get; set; }

    public override bool IsDefaultAttribute() => Equals(Default);
}