namespace Mantle.Web.Security.Membership;

public class MembershipSettings : BaseSettings
{
    public MembershipSettings()
    {
        GeneratedPasswordLength = 7;
        GeneratedPasswordNumberOfNonAlphanumericChars = 3;
    }

    [LocalizedDisplayName(MantleWebLocalizableStrings.Settings.Membership.GeneratedPasswordLength)]
    [SettingsProperty(7)]
    public byte GeneratedPasswordLength { get; set; }

    [LocalizedDisplayName(MantleWebLocalizableStrings.Settings.Membership.GeneratedPasswordNumberOfNonAlphanumericChars)]
    [SettingsProperty(3)]
    public byte GeneratedPasswordNumberOfNonAlphanumericChars { get; set; }

    [LocalizedDisplayName(MantleWebLocalizableStrings.Settings.Membership.DisallowUnconfirmedUserLogin)]
    [SettingsProperty]
    public bool DisallowUnconfirmedUserLogin { get; set; }

    #region ISettings Members

    public override string Name => "Membership Settings";

    public override string EditorTemplatePath => "/Views/Shared/EditorTemplates/MembershipSettings.cshtml";

    #endregion ISettings Members
}