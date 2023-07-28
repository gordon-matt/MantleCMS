namespace Mantle.Localization;

public interface ICultureManager
{
    string GetCurrentCulture();

    bool IsValidCulture(string cultureName);
}