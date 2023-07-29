namespace Mantle.Localization;

public partial class DefaultCultureManager : ICultureManager
{
    #region ICultureManager Members

    public string GetCurrentCulture()
    {
        return CultureInfo.CurrentUICulture.Name;
    }

    public bool IsValidCulture(string cultureName)
    {
        var cultureRegex = CultureRegex();
        if (cultureRegex.IsMatch(cultureName))
        {
            return true;
        }
        return false;
    }

    [GeneratedRegex("\\w{2}(-\\w{2,})*")]
    private static partial Regex CultureRegex();

    #endregion ICultureManager Members
}