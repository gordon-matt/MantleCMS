using System.ComponentModel.DataAnnotations;
using Mantle.ComponentModel;

namespace Mantle.Web.Configuration
{
    public class DateTimeSettings : ISettings
    {
        [Required]
        [LocalizedDisplayName(MantleWebLocalizableStrings.Settings.DateTime.DefaultTimeZoneId)]
        public string DefaultTimeZoneId { get; set; }

        [LocalizedDisplayName(MantleWebLocalizableStrings.Settings.DateTime.AllowUsersToSetTimeZone)]
        public bool AllowUsersToSetTimeZone { get; set; }

        #region ISettings Members

        public string Name
        {
            get { return "Date/Time Settings"; }
        }

        public bool IsTenantRestricted
        {
            get { return false; }
        }

        public string EditorTemplatePath
        {
            get { return "Mantle.Web.Views.Shared.EditorTemplates.DateTimeSettings"; }
        }

        #endregion ISettings Members
    }
}