namespace Mantle.Localization;

public partial class DefaultCultureManager : ICultureManager
{
    #region ICultureManager Members

    public string GetCurrentCulture() => CultureInfo.CurrentUICulture.Name;

    public bool IsValidCulture(string cultureName)
    {
        var cultureRegex = CultureRegex();
        return cultureRegex.IsMatch(cultureName);
    }

    [GeneratedRegex("\\w{2}(-\\w{2,})*")]
    private static partial Regex CultureRegex();

    #endregion ICultureManager Members
}