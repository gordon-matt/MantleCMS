using System.ComponentModel.DataAnnotations;

namespace Mantle.Web.Configuration;

public class DateTimeSettings : BaseSettings
{
    [Required]
    [LocalizedDisplayName(MantleWebLocalizableStrings.Settings.DateTime.DefaultTimeZoneId)]
    [SettingsProperty]
    public string DefaultTimeZoneId { get; set; }

    [LocalizedDisplayName(MantleWebLocalizableStrings.Settings.DateTime.AllowUsersToSetTimeZone)]
    [SettingsProperty]
    public bool AllowUsersToSetTimeZone { get; set; }

    #region ISettings Members

    public override string Name => "Date/Time Settings";

    public override string EditorTemplatePath => "/Views/Shared/EditorTemplates/DateTimeSettings.cshtml";

    #endregion ISettings Members
}