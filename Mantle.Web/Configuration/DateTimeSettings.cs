using System.ComponentModel.DataAnnotations;

namespace Mantle.Web.Configuration;

public class DateTimeSettings : ISettings
{
    [Required]
    [LocalizedDisplayName(MantleWebLocalizableStrings.Settings.DateTime.DefaultTimeZoneId)]
    public string DefaultTimeZoneId { get; set; }

    [LocalizedDisplayName(MantleWebLocalizableStrings.Settings.DateTime.AllowUsersToSetTimeZone)]
    public bool AllowUsersToSetTimeZone { get; set; }

    #region ISettings Members

    public string Name => "Date/Time Settings";

    public bool IsTenantRestricted => false;

    public string EditorTemplatePath => "Mantle.Web.Views.Shared.EditorTemplates.DateTimeSettings.cshtml";

    #endregion ISettings Members
}