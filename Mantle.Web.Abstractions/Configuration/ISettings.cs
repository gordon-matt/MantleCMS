namespace Mantle.Web.Configuration;

public interface ISettings
{
    string Name { get; }

    /// <summary>
    /// True if these settings are global (for all sites) and only the admin user can modify.
    /// False if each tenant can have their own customized settings.
    /// </summary>
    bool IsTenantRestricted { get; }

    string EditorTemplatePath { get; }
}