using Mantle.ComponentModel;
using Mantle.Web.Configuration;

namespace Mantle.Web.Security.Membership
{
    public class MembershipSettings : ISettings
    {
        public MembershipSettings()
        {
            GeneratedPasswordLength = 7;
            GeneratedPasswordNumberOfNonAlphanumericChars = 3;
        }

        [LocalizedDisplayName(MantleWebLocalizableStrings.Settings.Membership.GeneratedPasswordLength)]
        public byte GeneratedPasswordLength { get; set; }

        [LocalizedDisplayName(MantleWebLocalizableStrings.Settings.Membership.GeneratedPasswordNumberOfNonAlphanumericChars)]
        public byte GeneratedPasswordNumberOfNonAlphanumericChars { get; set; }

        [LocalizedDisplayName(MantleWebLocalizableStrings.Settings.Membership.DisallowUnconfirmedUserLogin)]
        public bool DisallowUnconfirmedUserLogin { get; set; }

        #region ISettings Members

        public string Name
        {
            get { return "Membership Settings"; }
        }

        public bool IsTenantRestricted
        {
            get { return false; }
        }

        public string EditorTemplatePath
        {
            get { return "Mantle.Web.Views.Shared.EditorTemplates.MembershipSettings"; }
        }

        #endregion ISettings Members
    }
}