namespace Mantle.Web.Security.Membership;

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

    public string Name => "Membership Settings";

    public bool IsTenantRestricted => false;

    public string EditorTemplatePath => "Mantle.Web.Views.Shared.EditorTemplates.MembershipSettings.cshtml";

    #endregion ISettings Members
}